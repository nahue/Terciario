using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.Models;
using ColegioTerciario.Models.Repositories;
using Rotativa.MVC;

namespace ColegioTerciario.Controllers
{
    public class ActaExamenController : Controller
    {
        private ColegioTerciarioContext db = new ColegioTerciarioContext();

        // GET: ActaExamen
        public ActionResult Index()
        {
            ViewBag.MATERIAS = new SelectList(db.Materias, "ID", "MATERIA_NOMBRE");
            var actas_examenes = db.Actas_Examenes
                .Include(a => a.ACTA_EXAMEN_TURNO_EXAMEN.TURNO_EXAMEN_CICLO)
                .Include(a => a.ACTA_EXAMEN_MATERIA)
                .Include(a => a.ACTA_EXAMEN_CARRERA)
                .Include(a => a.ACTA_EXAMEN_PRESIDENTE)
                .Include(a => a.ACTA_EXAMEN_VOCAL1)
                .Include(a => a.ACTA_EXAMEN_VOCAL2)
                .ToList();
            return View(actas_examenes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult agregarAlumnos(int ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID, int[] alumnos)
        {
            //TODO: guardar estado de cursada en tabla de actas finales
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (int personaId in alumnos)
                    {
                        if (db.Actas_Examenes_Detalles.Where(
                            a => a.ACTA_EXAMEN_DETALLE_ALUMNOS_ID == personaId
                            && a.ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID == ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID).Count() == 0)
                        { 
                        Acta_Examen_Detalle nuevoDetalle = new Acta_Examen_Detalle()
                            {
                                ACTA_EXAMEN_DETALLE_ALUMNOS_ID = personaId,
                                ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID = ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID
                            };
                            db.Actas_Examenes_Detalles.Add(nuevoDetalle);
                            db.SaveChanges();
                        }

                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    return RedirectToRoute(new System.Web.Routing.RouteValueDictionary() { 
                        {"Controller", "ActaExamen"}, 
                        {"Action", "Edit"},
                        {"id", ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID}
                    });
                }

            }
            return RedirectToRoute(new System.Web.Routing.RouteValueDictionary() { 
                {"Controller", "ActaExamen"}, 
                {"Action", "Edit"},
                {"id", ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID}
            });
        }

        [HttpPost]
        public JsonResult ponerNota(int pk, string value, string name)
        {
            Acta_Examen_Detalle detalle = db.Actas_Examenes_Detalles.Find(pk);
            detalle.ACTA_EXAMEN_DETALLE_NOTA = value;
            if (value != "Ausente" && int.Parse(value) >= 6)
            {
                detalle.ACTA_EXAMEN_DETALLE_ESTADO = "APROBADO";
            }
            else
            {
                detalle.ACTA_EXAMEN_DETALLE_ESTADO = "REPROBADO";
            }
            db.SaveChanges();

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        // GET: ActaExamen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acta_Examen acta_Examen = db.Actas_Examenes
                .Include(a => a.ACTA_EXAMEN_TURNO_EXAMEN.TURNO_EXAMEN_CICLO)
                .Include(a => a.ACTA_EXAMEN_CARRERA)
                .Include(a => a.ACTA_EXAMEN_MATERIA)
                .Include(a => a.ACTA_EXAMEN_PRESIDENTE)
                .Include(a => a.ACTA_EXAMEN_VOCAL1)
                .Include(a => a.ACTA_EXAMEN_VOCAL2)
                .SingleOrDefault(a => a.ID.Equals(id.Value));

            if (acta_Examen == null)
            {
                return HttpNotFound();
            }
            //var repo = new PersonasRepository();
            //ViewBag.ALUMNOS = repo.GetPersonasByActa(acta_Examen.ID);
            ViewBag.DETALLES = db.Actas_Examenes_Detalles
                .Include(a => a.ACTA_EXAMEN_DETALLE_ALUMNO)
                .Where(a => a.ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID == id)
                .OrderBy(a => a.ACTA_EXAMEN_DETALLE_ALUMNO.PERSONA_APELLIDO)
                .ToList();

            return new ViewAsPdf(acta_Examen);
        }

        // GET: ActaExamen/Create
        public ActionResult Create()
        {
            ViewBag.CARRERAS = new SelectList(db.Carreras, "ID", "CARRERA_NOMBRE");
            //ViewBag.MATERIAS = new SelectList(db.Materias, "ID", "MATERIA_NOMBRE");
            ViewBag.PERSONAS = new SelectList(db.Personas, "ID", "PERSONA_NOMBRE");
            ViewBag.TURNOS = new SelectList(db.Turnos_Examenes.Include(t => t.TURNO_EXAMEN_CICLO), "ID", "TURNO_EXAMEN_NOMBRE_PARA_MOSTRAR");
            return View();
        }

        // POST: ActaExamen/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ACTA_EXAMEN_FECHA,ACTA_EXAMEN_LIBRO,ACTA_EXAMEN_FOLIO,ACTA_EXAMEN_TURNOS_EXAMENES_ID,ACTA_EXAMEN_CARRERAS_ID,ACTA_EXAMEN_MATERIAS_ID,ACTA_EXAMEN_PRESIDENTE_ID,ACTA_EXAMEN_VOCAL1_ID,ACTA_EXAMEN_VOCAL2_ID")] Acta_Examen acta_Examen)
        {
            ViewBag.CARRERAS = new SelectList(db.Carreras, "ID", "CARRERA_NOMBRE");
            ViewBag.PERSONAS = new SelectList(db.Personas, "ID", "PERSONA_NOMBRE");
            ViewBag.TURNOS = new SelectList(db.Turnos_Examenes.Include(t => t.TURNO_EXAMEN_CICLO), "ID", "TURNO_EXAMEN_NOMBRE_PARA_MOSTRAR");
            
            if (ModelState.IsValid)
            {
                var actasExistentes = db.Actas_Examenes.Any(
                    a => a.ACTA_EXAMEN_TURNOS_EXAMENES_ID == acta_Examen.ACTA_EXAMEN_TURNOS_EXAMENES_ID &&
                         a.ACTA_EXAMEN_MATERIAS_ID == acta_Examen.ACTA_EXAMEN_MATERIAS_ID &&
                         a.ACTA_EXAMEN_FECHA == acta_Examen.ACTA_EXAMEN_FECHA
                    );
                if (actasExistentes)
                {
                    ModelState.AddModelError("ACTA_EXAMEN_FECHA", "Ya existe una mesa de esta materia para el turno seleccionado.");
                    return View(acta_Examen);
                }
                db.Actas_Examenes.Add(acta_Examen);
                db.SaveChanges();
                return RedirectToAction("Edit", new{id=acta_Examen.ID});
            }
            return View(acta_Examen);
        }

        // GET: ActaExamen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acta_Examen acta_Examen = db.Actas_Examenes.Find(id);

            if (acta_Examen == null)
            {
                return HttpNotFound();
            }
            ViewBag.CARRERAS = new SelectList(db.Carreras, "ID", "CARRERA_NOMBRE");
            ViewBag.PERSONAS = new SelectList(db.Personas, "ID", "PERSONA_NOMBRE");
            ViewBag.TURNOS = new SelectList(db.Turnos_Examenes.Include(t => t.TURNO_EXAMEN_CICLO), "ID", "TURNO_EXAMEN_NOMBRE_PARA_MOSTRAR");
            ViewBag.ALUMNOS = db.Actas_Examenes_Detalles
                .Include("ACTA_EXAMEN_DETALLE_ALUMNO")
                .Where(c => c.ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID == id)
                .OrderBy(a => a.ACTA_EXAMEN_DETALLE_ALUMNO.PERSONA_APELLIDO)
                .ToList();
            return View(acta_Examen);
        }

        // POST: ActaExamen/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ACTA_EXAMEN_FECHA,ACTA_EXAMEN_INSCRIPTOS,ACTA_EXAMEN_APROBADOS,ACTA_EXAMEN_REPROBADOS,ACTA_EXAMEN_AUSENTES,ACTA_EXAMEN_LIBRO,ACTA_EXAMEN_FOLIO,ACTA_EXAMEN_TURNOS_EXAMENES_ID,ACTA_EXAMEN_CARRERAS_ID,ACTA_EXAMEN_MATERIAS_ID,ACTA_EXAMEN_PRESIDENTE_ID,ACTA_EXAMEN_VOCAL1_ID,ACTA_EXAMEN_VOCAL2_ID,ACTA_EXAMEN_COMENTARIOS")] Acta_Examen acta_Examen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acta_Examen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CARRERAS = new SelectList(db.Carreras, "ID", "CARRERA_NOMBRE");
            ViewBag.PERSONAS = new SelectList(db.Personas, "ID", "PERSONA_NOMBRE");
            ViewBag.TURNOS = new SelectList(db.Turnos_Examenes.Include(t => t.TURNO_EXAMEN_CICLO), "ID", "TURNO_EXAMEN_NOMBRE_PARA_MOSTRAR");
            ViewBag.ALUMNOS = db.Actas_Examenes_Detalles
                .Include("ACTA_EXAMEN_DETALLE_ALUMNO")
                .Where(c => c.ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID == acta_Examen.ID)
                .OrderBy(a => a.ACTA_EXAMEN_DETALLE_ALUMNO.PERSONA_APELLIDO)
                .ToList();
            return View(acta_Examen);
        }

        // GET: ActaExamen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acta_Examen acta_Examen = db.Actas_Examenes.Find(id);
            if (acta_Examen == null)
            {
                return HttpNotFound();
            }
            return View(acta_Examen);
        }

        // POST: ActaExamen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Acta_Examen acta_Examen = db.Actas_Examenes.Find(id);
            db.Actas_Examenes.Remove(acta_Examen);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
