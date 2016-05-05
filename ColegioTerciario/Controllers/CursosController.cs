using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ColegioTerciario.Models;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.Models.Repositories;
using Newtonsoft.Json;
using Rotativa.MVC;
using ColegioTerciario.Lib;
using ColegioTerciario.Models.ViewModels;

namespace ColegioTerciario.Controllers
{
    public class CursosController : Controller
    {
        readonly ColegioTerciarioContext _db;

        public CursosController()
        {
            _db = new ColegioTerciarioContext();
        }

        // GET: Cursos
        public ActionResult Index(DataTableParamModel param)
        {
            return View();
        }

        /* GET: Cursos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }*/

        // GET: Cursos/Create
        public ActionResult Create()
        {
            ViewBag.CICLOS = new SelectList(_db.Ciclos, "ID", "CICLO_NOMBRE");
            //ViewBag.CARRERAS = new SelectList(_db.Carreras, "ID", "CARRERA_NOMBRE");
            ViewBag.CARRERAS = _db.Carreras.ToList();
            ViewBag.SEDES = new SelectList(_db.Sedes, "ID", "SEDE_NOMBRE");
            ViewBag.error = Session["error"];
            ViewBag.HORARIOS = _db.Horas.ToList();
            return View();
        }

        // POST: Cursos/agregarAlumnos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarAlumnos(int materiaXCursoId, int[] alumnos)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (int personaId in alumnos)
                    {
                        int id = personaId;
                        var cursadasConEstaPersona = _db.Cursadas.Count(c => c.CURSADA_ALUMNOS_ID == id && c.CURSADA_MATERIAS_X_CURSOS_ID == materiaXCursoId);
                        if (cursadasConEstaPersona == 0)
                        { 
                            var nuevaCursada = new Cursada
                            {
                                CURSADA_ALUMNOS_ID = personaId,
                                //CURSADA_NOTA_P1 = "Ausente",
                                //CURSADA_NOTA_P2 = "Ausente",
                                //CURSADA_NOTA_R1 = "Ausente",
                                //CURSADA_NOTA_R2 = "Ausente",
                                CURSADA_MATERIAS_X_CURSOS_ID = materiaXCursoId
                            };
                            _db.Cursadas.Add(nuevaCursada);
                            _db.SaveChanges();
                        }
                        
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    return RedirectToRoute(new System.Web.Routing.RouteValueDictionary
                    { 
                        {"Controller", "Cursos"}, 
                        {"Action", "Edit"},
                        {"id", materiaXCursoId}
                    });
                }
                
            }
            return RedirectToRoute(new System.Web.Routing.RouteValueDictionary
            { 
                {"Controller", "Cursos"}, 
                {"Action", "Edit"},
                {"id", materiaXCursoId}
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarAlumnosACurso(string ciclo, string nombre, int[] alumnos)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try { 
                var cursos = _db.Materias_X_Cursos.Include("MATERIA_X_CURSO_MATERIA").Where(
                   c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO == ciclo &&
                        c.MATERIA_X_CURSO_CURSO_NOMBRE == nombre
                   ).ToList();

                foreach(Materia_x_Curso curso in cursos)
                {
                    foreach(int alumnoId in alumnos)
                    {
                        int id = alumnoId;
                        int cursoId = curso.ID;
                        var cursadasConEstaPersona = _db.Cursadas.Count(
                            c => 
                                c.CURSADA_ALUMNOS_ID == id && 
                                c.CURSADA_MATERIAS_X_CURSOS_ID == cursoId);
                        if (cursadasConEstaPersona == 0)
                        {
                            var nuevaCursada = new Cursada
                            {
                                CURSADA_ALUMNOS_ID = alumnoId,
                                CURSADA_MATERIAS_X_CURSOS_ID = curso.ID,
                                //CURSADA_NOTA_P1 = "Ausente",
                                //CURSADA_NOTA_P2 = "Ausente",
                                //CURSADA_NOTA_R1 = "Ausente",
                                //CURSADA_NOTA_R2 = "Ausente",
                            };
                            _db.Cursadas.Add(nuevaCursada);
                            _db.SaveChanges();
                        }
                    }
                }
                transaction.Commit();
                return RedirectToRoute(new System.Web.Routing.RouteValueDictionary
                { 
                        {"Controller", "Cursos"}, 
                        {"Action", "editarCurso"},
                        {"ciclo", ciclo},
                        {"nombre", nombre}
                    });
                }
                catch (Exception)
                {
                    return RedirectToRoute(new System.Web.Routing.RouteValueDictionary
                    { 
                        {"Controller", "Cursos"}, 
                        {"Action", "editarCurso"},
                        {"ciclo", ciclo},
                        {"nombre", nombre}
                    });
                }

            }
        }

        [HttpPost]
        public JsonResult PonerNota(int pk, string value, string name)
        {
            Cursada cursada = _db.Cursadas.Find(pk);
            string nota = value != "Eliminar" ? value : null;
            
            switch (name)
            {
                case("P1"):
                    cursada.CURSADA_NOTA_P1 = nota;
                    break;
                case "R1":
                    cursada.CURSADA_NOTA_R1 = nota;
                    break;
                case "P2":
                    cursada.CURSADA_NOTA_P2 = nota;
                    if (cursada.CURSADA_NOTA_P1 != null && cursada.CURSADA_NOTA_P1 != "Ausente")
                    {
                        if (int.Parse(cursada.CURSADA_NOTA_P1) > 6 && int.Parse(nota) > 6)
                        {
                            cursada.CURSADA_ESTADO_ACADEMICO = "Regular";
                        }
                    }
                    break;
                case "R2":
                    cursada.CURSADA_NOTA_R2 = nota;
                    break;
            }
            
            _db.SaveChanges();

            var result = value != "Eliminar" ? "" : "Inserte la Nota";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Set(int pk, string value, string name)
        {
            Materia_x_Curso matXCurso = _db.Materias_X_Cursos.Find(pk);
            if (value != "")
            {
                switch (name)
                {
                    case ("MATERIA_X_CURSO_DOCENTE_ID"):
                        matXCurso.MATERIA_X_CURSO_DOCENTE_ID = int.Parse(value);
                        break;
                    case ("MATERIA_X_CURSO_DOCENTE_SUPLENTE_ID"):
                        matXCurso.MATERIA_X_CURSO_DOCENTE_SUPLENTE_ID = int.Parse(value);
                        break;
                    case ("MATERIA_X_CURSO_TURNO"):
                        matXCurso.MATERIA_X_CURSO_TURNO = value;
                        break;
                }
               
            }
            else
            {
                matXCurso.MATERIA_X_CURSO_DOCENTE_ID = null;
            }
            _db.SaveChanges();

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        // POST: Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(int SEDE, int CICLO, int CARRERA, int? AÑO, string[] NROS)
        public ActionResult Create(CreateCurso parametros)
        {
            if (parametros.Año == 0 || parametros.Nros.All(n => n == "false"))
            {
                Session["error"] = "Debe definir todos los campos";
                return RedirectToAction("create");
            }
           
            var materiasXCursos = new List<Materia_x_Curso>();
            var repo = new MateriasXCursoRepository(_db);

            


            try
            {
                Sede sede = _db.Sedes.Find(parametros.Sede);
                Ciclo ciclo = _db.Ciclos.Find(parametros.Ciclo);
                Carrera carrera = _db.Carreras.Find(parametros.Carrera);
                ICollection<Materia> materias = (from m in carrera.MATERIAS
                                                where m.MATERIA_ANIO == parametros.Año.ToString(CultureInfo.InvariantCulture)
                                                select m).ToList();
                
                
                foreach (Materia materia in materias)
                {
                    parametros.Nros = parametros.Nros.Where(nro => nro != "false").ToArray();
                    foreach (string nro in parametros.Nros)
                    {
                        materiasXCursos.Add(new Materia_x_Curso
                        {
                            MATERIA_X_CURSO_CARRERA = carrera,
                            MATERIA_X_CURSO_CICLO = ciclo,
                            MATERIA_X_CURSO_CURSO_NOMBRE = carrera.CARRERA_CODIGO + "-" + parametros.Año + "" + nro,
                            MATERIA_X_CURSO_MATERIA = materia,
                            MATERIA_X_CURSO_SEDE = sede
                        });
                    }
                }

                

                try
                {
                    Session["materias_x_cursos_ids"] = repo.InsertMateriasXCursos(materiasXCursos);
                }
                catch (Exception ex)
                {
                    Session["error"] = "El Curso ya existe con esta definición";
                    return RedirectToAction("create");
                }
                

                return RedirectToAction("Resumen");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Resumen()
        {
            if (Session["materias_x_cursos_ids"] != null)
            { 
                var ids = Session["materias_x_cursos_ids"] as ICollection<int>;
                ICollection<Materia_x_Curso> materiasXCursos = _db.Materias_X_Cursos.
                    Include("MATERIA_X_CURSO_CICLO").
                    Include("MATERIA_X_CURSO_CARRERA").
                    Include("MATERIA_X_CURSO_MATERIA").
                    Where(c => ids.Contains(c.ID)).ToList(); //(from m in db.Materias_X_Cursos where ids.Contains(m.ID) select m) .ToList();
                return View(materiasXCursos);
            }
            return Redirect("/");
            
        }

        [HttpGet]
        public ActionResult EditarCurso(string ciclo, string nombre, int? sedeId)
        {
            
            var cursos = _db.Materias_X_Cursos.Include("MATERIA_X_CURSO_MATERIA").Where(
                c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO == ciclo &&
                     c.MATERIA_X_CURSO_CURSO_NOMBRE == nombre &&
                     c.MATERIA_X_CURSO_SEDES_ID == sedeId
                ).ToList();
            ViewBag.CICLO = ciclo;
            ViewBag.NOMBRE = nombre;
            return View(cursos);
        }
        // GET: Cursos/Edit/5
        public ActionResult Edit(int id)
        {
            var materiasSelectList = new Dictionary<string, string>();
            var curso = _db.Materias_X_Cursos
                .Include("MATERIA_X_CURSO_CARRERA")
                .Include("MATERIA_X_CURSO_MATERIA")
                .Include("MATERIA_X_CURSO_CICLO")
                .Include("MATERIA_X_CURSO_DOCENTE")
                .Include("MATERIA_X_CURSO_DOCENTE_SUPLENTE")
                .FirstOrDefault(c => c.ID == id);

            var materias = _db.Materias_X_Cursos.Include("MATERIA_X_CURSO_CICLO").Include("MATERIA_X_CURSO_MATERIA").Where(
                c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO == curso.MATERIA_X_CURSO_CICLO.CICLO_ANIO &&
                     c.MATERIA_X_CURSO_CURSO_NOMBRE == curso.MATERIA_X_CURSO_CURSO_NOMBRE &&
                     c.MATERIA_X_CURSO_SEDES_ID == curso.MATERIA_X_CURSO_SEDES_ID
                ).Select(c => new {c.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE, c.MATERIA_X_CURSO_MATERIA.ID}).ToList();

            foreach (var materia in materias)
            {
                var a =
                    _db.Materias_X_Cursos.Include("MATERIA_X_CURSO_MATERIA")
                    .SingleOrDefault(c => c.MATERIA_X_CURSO_CURSO_NOMBRE == curso.MATERIA_X_CURSO_CURSO_NOMBRE &&
                        c.MATERIA_X_CURSO_CICLO.CICLO_ANIO == curso.MATERIA_X_CURSO_CICLO.CICLO_ANIO &&
                        c.MATERIA_X_CURSO_MATERIAS_ID == materia.ID && c.MATERIA_X_CURSO_SEDES_ID == curso.MATERIA_X_CURSO_SEDES_ID);
                materiasSelectList.Add(a.ID.ToString(), a.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE);
            }

            var cursadas = from c in _db.Cursadas
                orderby c.CURSADA_ALUMNO.PERSONA_APELLIDO
                where c.CURSADA_MATERIAS_X_CURSOS_ID == id && c.CURSADA_ALUMNO.PERSONA_DOCUMENTO_NUMERO != null
                select new CursadasViewModel()
                {
                    ID = c.ID,
                    AlumnoApellido = c.CURSADA_ALUMNO.PERSONA_APELLIDO,
                    AlumnoNombre = c.CURSADA_ALUMNO.PERSONA_NOMBRE,
                    AlumnoID = c.CURSADA_ALUMNO.ID,
                    DocumentoNumero = c.CURSADA_ALUMNO.PERSONA_DOCUMENTO_NUMERO,
                    EstadoAcademico = c.CURSADA_ESTADO_ACADEMICO,
                    EstadoAsistencia = c.CURSADA_ESTADO_ASISTENCIA,
                    EstadoDefinitivo = c.CURSADA_ESTADO_DEFINITIVO,
                    NotaP1 = c.CURSADA_NOTA_P1,
                    NotaP2 = c.CURSADA_NOTA_P2,
                    NotaR1 = c.CURSADA_NOTA_R1,
                    NotaR2 = c.CURSADA_NOTA_R2
                };

            ViewBag.alumnos = cursadas.ToList();
            ViewBag.HORARIOS = _db.Horas.ToList();
            var selectListMaterias = new SelectList(materiasSelectList,"Key", "Value", curso.ID);
            ViewBag.MATERIAS = selectListMaterias;

            ViewBag.lunes = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_LUNES);
            ViewBag.martes = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_MARTES);
            ViewBag.miercoles = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_MIERCOLES);
            ViewBag.jueves = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_JUEVES);
            ViewBag.viernes = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_VIERNES);
            ViewBag.sabado = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_SABADO);
            ViewBag.domingo = diaToArray(curso.MATERIA_X_CURSO_HORARIOS_DOMINGO);

            return View(curso);
        }

        private List<string> diaToArray(string dia)
        {
            if (dia != null)
            {
                return dia.Split(new Char[] {'-'}).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        // POST: Cursos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var curso = _db.Materias_X_Cursos.Find(id);
                curso.MATERIA_X_CURSO_HORARIOS_LUNES = collection["Lunes"] != null ? collection["Lunes"].Replace(",", "-") : "";
                curso.MATERIA_X_CURSO_HORARIOS_MARTES = collection["Martes"] != null ? collection["Martes"].Replace(",", "-") : "";
                curso.MATERIA_X_CURSO_HORARIOS_MIERCOLES = collection["Miercoles"] != null ? collection["Miercoles"].Replace(",", "-") : "";
                curso.MATERIA_X_CURSO_HORARIOS_JUEVES = collection["Jueves"] != null ? collection["Jueves"].Replace(",", "-") : "";
                curso.MATERIA_X_CURSO_HORARIOS_VIERNES = collection["Viernes"] != null ? collection["Viernes"].Replace(",", "-") : "";
                curso.MATERIA_X_CURSO_HORARIOS_SABADO = collection["Sabado"] != null ? collection["Sabado"].Replace(",", "-") : "";
                curso.MATERIA_X_CURSO_HORARIOS_DOMINGO = collection["Domingo"] != null ? collection["Domingo"].Replace(",", "-") : "";
                _db.SaveChanges();
                var currentUrl = Request.Url.AbsoluteUri;
                return Redirect(currentUrl);
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: Cursos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Cursos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult SetearFecha(int pk, DateTime? value, string name)
        {
            try
            {
                Materia_x_Curso curso = _db.Materias_X_Cursos.Find(pk);
                switch (name)
                {
                    case "P1_FECHA":
                        curso.MATERIA_X_CURSO_P1_FECHA = value;
                        
                        break;
                    case "R1_FECHA":
                        curso.MATERIA_X_CURSO_R1_FECHA = value;
                        break;
                    case "P2_FECHA":
                        curso.MATERIA_X_CURSO_P2_FECHA = value;
                        break;
                    case "R2_FECHA":
                        curso.MATERIA_X_CURSO_R2_FECHA = value;
                        break;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    {"error", ex.Message}
                });
                return Json(json, JsonRequestBehavior.AllowGet);
            }
           
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pdf(int id, string instancia, bool mostrarLibres = false)
        {
            const int notaMinima = 6;

            var curso = _db.Materias_X_Cursos
                .Include("MATERIA_X_CURSO_CARRERA")
                .Include("MATERIA_X_CURSO_MATERIA")
                .Include("MATERIA_X_CURSO_DOCENTE")
                .Include("MATERIA_X_CURSO_CICLO").Single(c => c.ID == id);

            var reporte = new ParcialPDF
            {
                Instancia = instancia,
                Ciclo = curso.MATERIA_X_CURSO_CICLO.CICLO_NOMBRE,
                Codigo = curso.MATERIA_X_CURSO_CURSO_NOMBRE,
                Integrantes = new List<Integrante>()
            };

            var cursadas = _db.Cursadas
                .Include("CURSADA_ALUMNO")
                .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                .Where(c => c.CURSADA_ALUMNO != null && c.CURSADA_MATERIAS_X_CURSOS_ID == id);

            if (!mostrarLibres)
            {
                cursadas = cursadas.Where(
                    c => (
                        c.CURSADA_ESTADO_ACADEMICO != "Libre" &&
                        c.CURSADA_ESTADO_ASISTENCIA != "Libre" &&
                        c.CURSADA_ESTADO_DEFINITIVO != "Libre"
                        )
                    );
            }

            foreach (var cursada in cursadas) {

                    var integrante = new Integrante();
                    
                    switch (reporte.Instancia)
                    {
                        case "P1":
                            reporte.Nombre = "Primer Parcial";
                            integrante.Calificacion = cursada.CURSADA_NOTA_P1;
                            reporte.Fecha = curso.MATERIA_X_CURSO_P1_FECHA;
                            break;
                        case "R1":
                            reporte.Nombre = "Primer Recuperatorio";
                            integrante.Calificacion = cursada.CURSADA_NOTA_R1;
                            reporte.Fecha = curso.MATERIA_X_CURSO_R1_FECHA;
                            break;
                        case "P2":
                            reporte.Nombre = "Segundo Parcial";
                            integrante.Calificacion = cursada.CURSADA_NOTA_P2;
                            reporte.Fecha = curso.MATERIA_X_CURSO_P2_FECHA;
                            break;
                        case "R2":
                            reporte.Nombre = "Segundo Recuperatorio";
                            integrante.Calificacion = cursada.CURSADA_NOTA_R2;
                            reporte.Fecha = curso.MATERIA_X_CURSO_R2_FECHA;
                            break;
                    }
                    
                    // Setea Ausente si no tiene nota
                    integrante.Calificacion = integrante.Calificacion ?? "Ausente";
                    integrante.Persona = cursada.CURSADA_ALUMNO;
                    reporte.Integrantes.Add(integrante);
            }
            reporte.Carrera = curso.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE;
            reporte.Docente = curso.MATERIA_X_CURSO_DOCENTE != null ? curso.MATERIA_X_CURSO_DOCENTE.PERSONA_NOMBRE_COMPLETO : "";
            reporte.Materia = curso.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE;
            reporte.Sede = curso.MATERIA_X_CURSO_SEDE.SEDE_NOMBRE;
            
            reporte.Inscriptos = reporte.Integrantes.Count();

            reporte.Examinados = reporte.Integrantes.Count(a => a.Calificacion != null);
            reporte.Aprobados = reporte.Integrantes.Count(a => a.Calificacion.ToNullableInt32() >= notaMinima);
            reporte.Desaprobados = reporte.Integrantes.Count(a => a.Calificacion.ToNullableInt32() < notaMinima);
            reporte.Ausentes = reporte.Integrantes.Count(a => a.Calificacion == "Ausente");

            return new ViewAsPdf(reporte);
        }
    }
}
