using ClosedXML.Excel;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.Models;
using ColegioTerciario.Models.ViewModels;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;
using Rotativa.MVC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ColegioTerciario.Controllers
{
    public class AsistenciasController : Controller
    {
        private ColegioTerciarioContext db = new ColegioTerciarioContext();

        public ActionResult Edit(int cursoId, string fecha)
        {
            DateTime date = Convert.ToDateTime(fecha);
            IEnumerable<Asistencia> asistencia = db.Asistencias.Include("ALUMNO").Where(a => a.MATERIA_X_CURSOS_ID == cursoId && a.ASISTENCIA_FECHA == date).ToList();
            ViewBag.hoy = asistencia.First().ASISTENCIA_FECHA;
            return View(asistencia);
        }

        public ActionResult ImprimirPlanillaVacia(int? cursoId, string tipo)
        {
            try
            {
                Materia_x_Curso curso = db.Materias_X_Cursos
                                    .Include("MATERIA_X_CURSO_CICLO")
                                    .Include("MATERIA_X_CURSO_CARRERA")
                                    .Include("MATERIA_X_CURSO_MATERIA")
                                    .Include("MATERIA_X_CURSO_DOCENTE")
                                    .FirstOrDefault(m => m.ID == cursoId);
                var alumnos = new List<Persona>();
                IEnumerable<Cursada> cursadas = db.Cursadas
                    .Include("CURSADA_ALUMNO")
                    .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                    .Where(c => c.CURSADA_MATERIAS_X_CURSOS_ID == cursoId && c.CURSADA_ALUMNO != null);//.ToList();

                if (tipo == "regulares")
                {
                    cursadas = cursadas.Where(
                       c => c.CURSADA_ESTADO_ASISTENCIA != "Libre").ToList();
                }

                foreach (Cursada cursada in cursadas)
                {
                    alumnos.Add(cursada.CURSADA_ALUMNO);
                }

                ViewBag.CICLO = curso.MATERIA_X_CURSO_CICLO.CICLO_ANIO;
                ViewBag.CARRERA = curso.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE;
                ViewBag.CODIGO = curso.MATERIA_X_CURSO_CURSO_NOMBRE;
                ViewBag.MATERIA = curso.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE;

                if (curso.MATERIA_X_CURSO_DOCENTE == null)
                {
                    var referrer = Request.UrlReferrer.AbsoluteUri;
                    Flash.Instance.Error("Error", "Debe especificar el Docente de este curso para poder tomar asistencia.");
                    return Redirect(referrer);
                }

                ViewBag.DOCENTE = curso.MATERIA_X_CURSO_DOCENTE.PERSONA_NOMBRE_COMPLETO;

                return new ViewAsPdf(alumnos);
            }
            catch (Exception ex)
            {
                var referrer = Request.UrlReferrer.AbsoluteUri;
                Flash.Instance.Error("Error", ex.Message);
                return Redirect(referrer);
                throw;
            }
        }

        public ActionResult Index(int? cursoId)
        {
            Materia_x_Curso curso = db.Materias_X_Cursos.Include("MATERIA_X_CURSO_CICLO").FirstOrDefault(m => m.ID == cursoId);

            ViewBag.CURSO_ID = curso.ID;
            ViewBag.CURSO = curso.MATERIA_X_CURSO_CURSO_NOMBRE;
            ViewBag.CICLO = curso.MATERIA_X_CURSO_CICLO.CICLO_NOMBRE;
            var asistencias = from a in db.Asistencias
                              where a.MATERIA_X_CURSOS_ID == cursoId
                              group a by a.ASISTENCIA_FECHA
                                  into grp
                                  select new AsistenciasViewModel()
                                  {
                                      ASISTENCIA_ID = grp.FirstOrDefault().ID,
                                      ASISTENCIA_FECHA = grp.Key
                                  };

            return View(asistencias);
        }

        // GET: Asistencias
        public ActionResult Nueva(int? cursoId)
        {
            ViewBag.hoy = DateTime.Today;
            Materia_x_Curso curso = db.Materias_X_Cursos.Find(cursoId);
            List<Persona> alumnos = new List<Persona>();
            IEnumerable<Cursada> cursadas = db.Cursadas
                .Include("CURSADA_ALUMNO")
                .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                .Where(c => c.CURSADA_MATERIAS_X_CURSOS_ID == curso.ID && c.CURSADA_ALUMNO != null).ToList();

            foreach (Cursada cursada in cursadas)
            {
                alumnos.Add(cursada.CURSADA_ALUMNO);
            }
            ViewBag.alumnos = alumnos;
            ViewBag.NEXT = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : "/Cursos";
            return View();
        }

        [HttpPost]
        public ActionResult Nueva(int? cursoId, FormCollection collection)
        {
            if (!collection.AllKeys.Contains("asistencia[]"))
            {
                Flash.Instance.Error("Asistencias", "Debe elegir por lo menos un alumno");
                return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : collection["NEXT"]);
            }

            string[] presentes = collection["asistencia[]"].Split(',');
            var fechaString = collection["FECHA"];

            //var curso = db.Materias_X_Cursos.Find(cursoId);
            //var url = "/Curso/" + curso.MATERIA_X_CURSO_CICLO.CICLO_ANIO + "/" + curso.

            var fecha = DateTime.ParseExact(fechaString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (db.Asistencias.Any(a => a.ASISTENCIA_FECHA == fecha && a.MATERIA_X_CURSOS_ID == cursoId))
            {
                Flash.Instance.Error("Asistencias", "Ya se tomó asistencia para este curso");
                return Redirect(collection["NEXT"]);
            }
            IEnumerable<Cursada> cursadas = db.Cursadas
                .Include("CURSADA_ALUMNO")
                .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                .Where(c => c.CURSADA_MATERIAS_X_CURSOS_ID == cursoId).ToList();

            foreach (Cursada cursada in cursadas)
            {
                Persona alumno = cursada.CURSADA_ALUMNO;
                var asistencia = new Asistencia();
                asistencia.ASISTENCIA_FECHA = fecha;
                asistencia.ASISTENCIA_ALUMNO_ID = cursada.CURSADA_ALUMNOS_ID;
                asistencia.MATERIA_X_CURSOS_ID = cursoId;
                bool presente = presentes.Contains(cursada.CURSADA_ALUMNOS_ID.ToString());
                if (presente)
                {
                    asistencia.ASISTENCIA_PRESENTE = true;
                }

                db.Asistencias.Add(asistencia);
                db.SaveChanges();
            }

            Flash.Instance.Success("Asistencias", "Se guardaron correctamente las Asistencias");
            return Redirect(collection["NEXT"]);
        }

        [HttpGet]
        public ActionResult Exportar(int? id, string format = "json")
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ALUMNO", typeof(string));
            var q = db.Asistencias
                            .Include("ALUMNO")
                            .Include("MATERIA_X_CURSO")
                            .Where(a => a.MATERIA_X_CURSOS_ID == id && a.ALUMNO != null);
            var fechas = q
                        .GroupBy(a => a.ASISTENCIA_FECHA)
                        .OrderBy(a => a.Key)
                        .Select(a => a.Key).ToList();
            foreach (var fecha in fechas)
            {
                dt.Columns.Add(fecha.ToString("dd/MM/yyyy", new CultureInfo("es-AR")), typeof(string));
            }


            var alumnos = q
                        .GroupBy(a => a.ALUMNO)
                        .OrderBy(a => a.Key.PERSONA_APELLIDO)
                        .Select(a => new
                        {
                            ID = a.Key.ID,
                            NOMBRE = a.Key.PERSONA_APELLIDO + ", " + a.Key.PERSONA_NOMBRE + " - " + a.Key.PERSONA_DOCUMENTO_NUMERO,
                            DOCUMENTO = a.Key.PERSONA_DOCUMENTO_NUMERO
                        }).ToList();

            foreach (var alumno in alumnos)
            {
                DataRow dr = dt.NewRow();
                dr["ALUMNO"] = alumno.NOMBRE;
                foreach (var fecha in fechas)
                {
                    var presente = q.Where(a => a.ASISTENCIA_ALUMNO_ID == alumno.ID && a.ASISTENCIA_FECHA == fecha)
                            .OrderBy(a => a.ASISTENCIA_FECHA)
                            .SingleOrDefault(p => p.ASISTENCIA_PRESENTE);

                    if (presente == null)
                    {
                        dr[fecha.ToString("dd/MM/yyyy", new CultureInfo("es-AR"))] = "";
                    }
                    else
                    {
                        dr[fecha.ToString("dd/MM/yyyy", new CultureInfo("es-AR"))] = presente.ASISTENCIA_PRESENTE ? "P" : "A";
                    }

                }

                dt.Rows.Add(dr);

            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.AddWorksheet(dt, "Asistencias");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Asistencias.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    var os = Response.OutputStream;
                    Response.Flush();
                    Response.End();
                }
            }

            return null;
        }
    }
}