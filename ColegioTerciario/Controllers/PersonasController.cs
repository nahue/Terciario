using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.Models;
using ColegioTerciario.Models.Repositories;
using ColegioTerciario.Models.ViewModels;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;
using PagedList;
using Rotativa.MVC;
using RazorPDF;
using Rotativa.Core.Options;

namespace ColegioTerciario.Controllers
{
    public class PersonasController : Controller
    {
        private readonly ColegioTerciarioContext _db;
        private readonly PersonasRepository _repo;

        public PersonasController()
        {
            _db = new ColegioTerciarioContext();
            _repo = new PersonasRepository();
        }

        // GET: Personas
        
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.EmailSortParam = sortOrder;
            ViewBag.DomicilioSortParm = sortOrder;
            ViewBag.TelefonoSortParm = sortOrder;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = sortOrder == "nombre" ? "nombre_desc" : "nombre";
            ViewBag.ApellidoSortParm = sortOrder == "apellido" ? "apellido_desc" : "apellido";
            ViewBag.DocSortParm = sortOrder == "documento" ? "documento_desc" : "documento";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var personas = from s in _db.Personas
                           select s;
            
            //busqueda
            if (!String.IsNullOrEmpty(searchString))
            {
                personas = personas.Where(s => s.PERSONA_NOMBRE.ToUpper().Contains(searchString.ToUpper())
                                            || s.PERSONA_APELLIDO.ToUpper().Contains(searchString.ToUpper())
                                            || s.PERSONA_DOCUMENTO_NUMERO.ToUpper().Contains(searchString.ToUpper())
                                       );
            }

            //Orden
            switch (sortOrder)
            {
                case "nombre":
                    personas = personas.OrderBy(s => s.PERSONA_NOMBRE);
                    break;
                case "nombre_desc":
                    personas = personas.OrderByDescending(s => s.PERSONA_NOMBRE);
                    break;
                case "documento":
                    personas = personas.OrderBy(s => s.PERSONA_DOCUMENTO_NUMERO);
                    break;
                case "documento_desc":
                    personas = personas.OrderByDescending(s => s.PERSONA_DOCUMENTO_NUMERO);
                    break;
                case "apellido_desc":
                    personas = personas.OrderByDescending(s => s.PERSONA_APELLIDO);
                    break;
                default:
                    personas = personas.OrderBy(s => s.PERSONA_APELLIDO); //por defecto orden por apellido ascendente
                    break;
            }
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(personas.ToPagedList(pageNumber, pageSize));
        }

        public JsonResult IndexJSON(DataTableParamModel param)
        {
            var personas = _db.Personas;
            List<Persona> personasFiltradas;

            if (param.sSearch == null)
            {
                personasFiltradas = personas.ToList();
            }
            else
            {
                personasFiltradas = (from e in personas
                                    where (
                                    e.PERSONA_DOCUMENTO_NUMERO.ToLower().Contains(param.sSearch.ToLower()) ||
                                    e.PERSONA_NOMBRE.ToLower().Contains(param.sSearch.ToLower()) ||
                                    e.PERSONA_APELLIDO.ToLower().Contains(param.sSearch.ToLower()))
                                    select e).ToList();
            }
            var result = from p in personasFiltradas.Skip(param.iDisplayStart)
                         .Take(param.iDisplayLength)
                         select new[]  {
                             Convert.ToString(p.ID),
                             p.PERSONA_NOMBRE,
                             p.PERSONA_APELLIDO,
                             p.PERSONA_DOCUMENTO_NUMERO
                         };
            
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = personas.Count(),
                iTotalDisplayRecords = personasFiltradas.Count,
                iDisplayStart = param.iDisplayStart,
                iDisplayLength = param.iDisplayLength,
                aaData = result
            },
            JsonRequestBehavior.AllowGet);

        }

        // GET: Personas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = _db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: Personas/Create
        public ActionResult Create()
        {
            /*
            ViewBag.PERSONA_NACIMIENTO_BARRIO_ID = new SelectList(_db.Barrios, "ID", "BARRIO_NOMBRE");
            ViewBag.PERSONA_NACIMIENTO_CIUDAD_ID = new SelectList(_db.Ciudades, "ID", "CIUDAD_NAME");
            ViewBag.PERSONA_NACIMIENTO_PAIS_ID = new SelectList(_db.Paises, "ID", "PAIS_NAME");
            ViewBag.PERSONA_NACIMIENTO_PROVINCIA_ID = new SelectList(_db.Provincias, "ID", "PROVINCIA_NAME_ASCII");
             * */
            ViewBag.PERSONA_NACIMIENTO_BARRIO_ID = new SelectList(_db.Barrios, "ID", "BARRIO_NOMBRE");
            ViewBag.PERSONA_NACIMIENTO_CIUDAD_ID = new SelectList(_db.Ciudades, "ID", "CIUDAD_NAME");
            ViewBag.PERSONA_NACIMIENTO_PAIS_ID = new SelectList(_db.Paises, "ID", "PAIS_NAME");
            ViewBag.PERSONA_NACIMIENTO_PROVINCIA_ID = new SelectList(_db.Provincias, "ID", "PROVINCIA_NAME_ASCII");
            var persona = new Persona();
            return View(persona);
        }

        // POST: Personas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PERSONA_CODIGO,PERSONA_USUARIO,PERSONA_CLAVE,PERSONA_NOMBRE,PERSONA_APELLIDO,PERSONA_NOMBRE_PARA_MOSTRAR,PERSONA_DOCUMENTO_TIPO,PERSONA_DOCUMENTO_NUMERO,PERSONA_NACIMIENTO_FECHA,PERSONA_EMAIL,PERSONA_DOMICILIO,PERSONA_TELEFONO,PERSONA_SEXO,PERSONA_FECHA_ALTA,PERSONA_FECHA_BAJA,PERSONA_TITULO_SECUNDARIO,PERSONA_OBSERVACION,PERSONA_FOTO,PERSONA_CUIL,PERSONA_ES_ALUMNO,PERSONA_ES_DOCENTE,PERSONA_ES_NODOCENTE,PERSONA_NACIMIENTO_PAIS_ID,PERSONA_NACIMIENTO_PROVINCIA_ID,PERSONA_NACIMIENTO_CIUDAD_ID,PERSONA_NACIMIENTO_BARRIO_ID")] Persona persona)
        {
            if (_db.Personas.Count(p => p.PERSONA_DOCUMENTO_NUMERO.Trim() == persona.PERSONA_DOCUMENTO_NUMERO.Trim()) >
                0)
            {
                ModelState.AddModelError("", "Ya existe una persona con este documento");
            }
            if (ModelState.IsValid)
            {
                _db.Personas.Add(persona);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = persona.ID });
            }
            
            ViewBag.PERSONA_NACIMIENTO_BARRIO_ID = new SelectList(_db.Barrios, "ID", "BARRIO_NOMBRE", persona.PERSONA_NACIMIENTO_BARRIO_ID);
            ViewBag.PERSONA_NACIMIENTO_CIUDAD_ID = new SelectList(_db.Ciudades, "ID", "CIUDAD_NAME", persona.PERSONA_NACIMIENTO_CIUDAD_ID);
            ViewBag.PERSONA_NACIMIENTO_PAIS_ID = new SelectList(_db.Paises, "ID", "PAIS_NAME", persona.PERSONA_NACIMIENTO_PAIS_ID);
            ViewBag.PERSONA_NACIMIENTO_PROVINCIA_ID = new SelectList(_db.Provincias, "ID", "PROVINCIA_NAME_ASCII", persona.PERSONA_NACIMIENTO_PROVINCIA_ID);
            return View(persona);
        }

        // GET: Personas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = _db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.PERSONA_NACIMIENTO_BARRIO_ID = new SelectList(_db.Barrios, "ID", "BARRIO_NOMBRE", persona.PERSONA_NACIMIENTO_BARRIO_ID);
            ViewBag.PERSONA_NACIMIENTO_CIUDAD_ID = new SelectList(_db.Ciudades, "ID", "CIUDAD_NAME", persona.PERSONA_NACIMIENTO_CIUDAD_ID);
            ViewBag.PERSONA_NACIMIENTO_PAIS_ID = new SelectList(_db.Paises, "ID", "PAIS_NAME", persona.PERSONA_NACIMIENTO_PAIS_ID);
            ViewBag.PERSONA_NACIMIENTO_PROVINCIA_ID = new SelectList(_db.Provincias, "ID", "PROVINCIA_NAME_ASCII", persona.PERSONA_NACIMIENTO_PROVINCIA_ID);
            if (persona.PERSONA_ES_ALUMNO == true)
            {
                ViewBag.SITUACIONPORCICLOS = _repo.GetSituacionAcademicaPorCiclos(persona).ToList();
                ViewBag.SITUACIONPORMATERIAS = _repo.GetSituacionAcademicaPorMaterias(persona).ToList();
                ViewBag.FINALES = _repo.GetFinales(persona).ToList();
                var equivalencias = _repo.GetEquivalencias(persona);
                ViewBag.EQUIVALENCIAS = equivalencias;

                if (equivalencias != null)
                {
                    ViewBag.EQUIVALENCIA_ID = equivalencias.First().EQUIVALENCIA_ID;
                }
            }
            return View(persona);
        }

        // POST: Personas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PERSONA_CODIGO,PERSONA_USUARIO,PERSONA_CLAVE,PERSONA_NOMBRE,PERSONA_APELLIDO,PERSONA_NOMBRE_PARA_MOSTRAR,PERSONA_DOCUMENTO_TIPO,PERSONA_DOCUMENTO_NUMERO,PERSONA_NACIMIENTO_FECHA,PERSONA_EMAIL,PERSONA_DOMICILIO,PERSONA_TELEFONO,PERSONA_SEXO,PERSONA_FECHA_ALTA,PERSONA_FECHA_BAJA,PERSONA_TITULO_SECUNDARIO,PERSONA_OBSERVACION,PERSONA_FOTO,PERSONA_CUIL,PERSONA_ES_ALUMNO,PERSONA_ES_DOCENTE,PERSONA_ES_NODOCENTE,PERSONA_NACIMIENTO_PAIS_ID,PERSONA_NACIMIENTO_PROVINCIA_ID,PERSONA_NACIMIENTO_CIUDAD_ID,PERSONA_NACIMIENTO_BARRIO_ID")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(persona).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = persona.ID });
            }
            ViewBag.PERSONA_NACIMIENTO_BARRIO_ID = new SelectList(_db.Barrios, "ID", "BARRIO_NOMBRE", persona.PERSONA_NACIMIENTO_BARRIO_ID);
            ViewBag.PERSONA_NACIMIENTO_CIUDAD_ID = new SelectList(_db.Ciudades, "ID", "CIUDAD_NAME", persona.PERSONA_NACIMIENTO_CIUDAD_ID);
            ViewBag.PERSONA_NACIMIENTO_PAIS_ID = new SelectList(_db.Paises, "ID", "PAIS_NAME", persona.PERSONA_NACIMIENTO_PAIS_ID);
            ViewBag.PERSONA_NACIMIENTO_PROVINCIA_ID = new SelectList(_db.Provincias, "ID", "PROVINCIA_NAME_ASCII", persona.PERSONA_NACIMIENTO_PROVINCIA_ID);
            return View(persona);
        }

        // GET: Personas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = _db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Persona persona = _db.Personas.Find(id);
            _db.Personas.Remove(persona);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ImprimirCertificadoAlumnoRegular(int? id)
        {
            Persona alumno = _db.Personas.Find(id);
            var repo = new PersonasRepository();
            
            if (repo.EsAlumnoRegular(alumno))
            {
                var hoy = DateTime.Now;
                var corriente = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                var ciclo = _db.Ciclos.SingleOrDefault(c => c.CICLO_ANIO == corriente);

                ViewBag.DIA = hoy.ToString("dd");
                ViewBag.MES = hoy.ToString("MMMM");
                ViewBag.AÑO = hoy.ToString("yyyy");

                var cursada = _db.Cursadas.Include("CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CARRERA").First(c =>
                            c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CICLOS_ID == ciclo.ID &&
                            c.CURSADA_ALUMNOS_ID == id);

                if (cursada != null) ViewBag.CARRERA = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CARRERA;
                if (ciclo != null) ViewBag.CICLO = ciclo.CICLO_NOMBRE;

                ViewBag.MEMBRETE = "Las Islas Malvinas, Georgias y Sandwich del Sur son y serán Argentinas";
                return new ViewAsPdf(alumno);
            }

            Flash.Instance.Error("Imprimir Certificado", "El alumno seleccionado no es regular");
            return RedirectToAction("Index");
        }

        public ActionResult ImprimirAnalitico(int? id, int? carreraID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = _db.Personas.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            var finales = _repo.GetAnalitico(persona, carreraID);
            var model = new AnaliticoViewModel
            {
                Persona = persona,
                Analitico = finales,
                Carrera = _db.Carreras.SingleOrDefault(c => c.ID == carreraID).CARRERA_NOMBRE
            };

            ViewBag.MEMBRETE = "2016, Año del Bicentenario de la Declaración de la Independencia Nacional.";

            return new ViewAsPdf("ImprimirAnalitico")
            {
                Model = model,
                RotativaOptions =
                {
                    //PageMargins = { Top = 5 },
                    PageSize = Size.Legal
                }
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
