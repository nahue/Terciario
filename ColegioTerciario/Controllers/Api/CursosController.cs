using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.Models;
using ColegioTerciario.Models.ViewModels;
using ColegioTerciario.Models.ViewModels.Api;
using System.Net.Http.Formatting;
using System.Globalization;

namespace ColegioTerciario.Controllers.Api
{
    public class FechaViewModel
    {
        public DateTime? value { get; set; }
        public int pk { get; set; }
        public string name { get; set; }
    }
    public class CursosController : ApiController
    {
        private readonly ColegioTerciarioContext _db;

        public CursosController()
        {
            _db = new ColegioTerciarioContext();
        }

        // GET: api/Cursos

        [HttpGet]
        public object GetCursos([FromUri]DataTableParamModel param)
        {
            var cursos = _db.Materias_X_Cursos
                   .Include("MATERIA_X_CURSO_CARRERA")
                   .Include("MATERIA_X_CURSO_CICLO")
                   .Include("MATERIA_X_CURSO_SEDE")
                   .Select(c => new
                   {
                       c.MATERIA_X_CURSO_CARRERA,
                       c.MATERIA_X_CURSO_CICLO,
                       c.MATERIA_X_CURSO_CURSO_NOMBRE,
                       c.MATERIA_X_CURSO_SEDE
                   })
                   .Distinct()
                   .ToList();
            var cursosFiltrados = (from c in cursos
                                   where (param.sSearch == null ||
                                   c.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE.ToLower().Contains(param.sSearch.ToLower()) ||
                                   c.MATERIA_X_CURSO_CICLO.CICLO_NOMBRE.ToLower().Contains(param.sSearch.ToLower()) ||
                                   c.MATERIA_X_CURSO_CURSO_NOMBRE.ToLower().Contains(param.sSearch.ToLower())
                                   )
                                   select c).ToList();

            var result = from c in cursosFiltrados.Skip(param.iDisplayStart)
                         .Take(param.iDisplayLength)
                         select new[]  {
                             c.MATERIA_X_CURSO_CICLO.CICLO_ANIO,
                             c.MATERIA_X_CURSO_SEDE != null ? c.MATERIA_X_CURSO_SEDE.ID.ToString() : null,
                             c.MATERIA_X_CURSO_CARRERA != null ? c.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE : null,                       
                             c.MATERIA_X_CURSO_CURSO_NOMBRE,
                             c.MATERIA_X_CURSO_SEDE.SEDE_NOMBRE
                         };

            return new
            {
                sEcho = param.sEcho,
                iTotalRecords = cursos.Count,
                iTotalDisplayRecords = cursosFiltrados.Count,
                iDisplayStart = param.iDisplayStart,
                iDisplayLength = param.iDisplayLength,
                aaData = result
            };
        }

        [HttpGet]
        public AjaxCollectionResponseViewModel ObtenerCursos([FromUri]AjaxCollectionParamViewModel param, [FromUri] int? docenteId)
        {

            var cursos = _db.Materias_X_Cursos
                .Where(
                    m => m.MATERIA_X_CURSO_DOCENTE_ID == docenteId && !m.MATERIA_X_CURSO_DEFINITIVO_EN_LIBRO
                )
                .OrderByDescending(m => m.MATERIA_X_CURSO_CICLO.CICLO_ANIO)
                .Skip(param.Pagina*param.RegistrosPorPagina)
                .Take(param.RegistrosPorPagina);
            /*
            
            */
            if (param.OrdenarPorColumna != null)
            {
                switch (param.OrdenarPorColumna)
                {
                    case "CICLO_ANIO":
                        cursos = param.OrdenarAsc ? cursos.OrderBy(c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO) : cursos.OrderByDescending(c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO);
                        break;
                    case "CARRERA_NOMBRE":
                        cursos = param.OrdenarAsc ? cursos.OrderBy(c => c.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE_CORTO) : cursos.OrderByDescending(c => c.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE_CORTO);
                        break;
                    case "MATERIA_X_CURSO_CURSO_NOMBRE":
                        cursos = param.OrdenarAsc ? cursos.OrderBy(c => c.MATERIA_X_CURSO_CURSO_NOMBRE) : cursos.OrderByDescending(c => c.MATERIA_X_CURSO_CURSO_NOMBRE);
                        break;
                    case "SEDE_NOMBRE":
                        cursos = param.OrdenarAsc ? cursos.OrderBy(c => c.MATERIA_X_CURSO_SEDE.SEDE_NOMBRE) : cursos.OrderByDescending(c => c.MATERIA_X_CURSO_SEDE.SEDE_NOMBRE);
                        break;
                    case "MATERIA_NOMBRE":
                        cursos = param.OrdenarAsc ? cursos.OrderBy(c => c.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE) : cursos.OrderByDescending(c => c.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE);
                        break;
                }
            }
            else
            {
                cursos = param.OrdenarAsc ? cursos.OrderBy(c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO) : cursos.OrderByDescending(c => c.MATERIA_X_CURSO_CICLO.CICLO_ANIO);
            }
            return new AjaxCollectionResponseViewModel
            {
                CantidadResultados = _db.Materias_X_Cursos.Count(m => m.MATERIA_X_CURSO_DOCENTE_ID == docenteId),
                Resultados = cursos.Select(c =>  new CursosPorDocenteViewModel {
                            ID = c.ID,
                            CICLO_ANIO = c.MATERIA_X_CURSO_CICLO.CICLO_ANIO,
                            MATERIA_X_CURSO_SEDES_ID = c.MATERIA_X_CURSO_SEDES_ID,
                            CARRERA_NOMBRE = c.MATERIA_X_CURSO_CARRERA != null ? c.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE_CORTO : null,                       
                            MATERIA_X_CURSO_CURSO_NOMBRE = c.MATERIA_X_CURSO_CURSO_NOMBRE,
                            SEDE_NOMBRE = c.MATERIA_X_CURSO_SEDE.SEDE_NOMBRE,
                            MATERIA_NOMBRE = c.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE,
                            CANTIDAD_PARCIALES = c.MATERIA_X_CURSO_CANTIDAD_PARCIALES
                        })
            };
        }

        [HttpGet]
        public IHttpActionResult ObtenerParcial([FromUri]int cursoId,[FromUri] string parcial)
        {
            var alumnos = new List<AlumnoEnCursadaViewModel>();
            string nota = "";
            bool es_regular = false;
            bool incluirEnResultado = true;

            var cursadas = _db.Cursadas.Include("CURSADA_ALUMNO")
                .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                .Where(c => 
                    c.CURSADA_ALUMNO != null && 
                    c.CURSADA_MATERIAS_X_CURSOS_ID == cursoId
                ).ToList();

            foreach (var cursada in cursadas)
            {
                incluirEnResultado = true;
                switch (parcial)
                {
                    case "P1":
                        nota = cursada.CURSADA_NOTA_P1;break;
                    case "P2":/*
                        if (cursada.CURSADA_NOTA_P1 == null)
                        {
                            es_regular = false;
                        }
                        else
                        {
                            if (int.Parse(cursada.CURSADA_NOTA_P1) > 6)
                            {
                                es_regular = true;
                            }
                            else if (int.Parse(cursada.CURSADA_NOTA_R1) > 6)
                            {
                                es_regular = true;
                            }
                            else
                            {
                                es_regular = false;
                            }
                        }*/
                        incluirEnResultado = cursada.CURSADA_P1_REGULAR;
                        nota = cursada.CURSADA_NOTA_P2;
                        break;
                    case "R1":
                        incluirEnResultado = !cursada.CURSADA_P1_REGULAR;
                        nota = cursada.CURSADA_NOTA_R1;
                        //if (cursada.CURSADA_NOTA_P1 == "Ausente" || cursada.CURSADA_NOTA_P1 == null)
                        //{
                        //    incluirEnResultado = false;
                        //}
                        //else if (int.Parse(cursada.CURSADA_NOTA_P1) > 6)
                        //{
                        //    incluirEnResultado = false;
                        //}
                        
                        break;
                    case "R2":
                        incluirEnResultado = !cursada.CURSADA_P2_REGULAR;
                        //if (cursada.CURSADA_NOTA_P2 == "Ausente" || cursada.CURSADA_NOTA_P2 == null)
                        //{
                        //    incluirEnResultado = false;
                        //}
                        //else if (int.Parse(cursada.CURSADA_NOTA_P2) > 6)
                        //{
                        //    incluirEnResultado = false;
                        //}



                        nota = cursada.CURSADA_NOTA_R2;break;

                }

                if (incluirEnResultado)
                {
                    alumnos.Add(new AlumnoEnCursadaViewModel
                    {
                        CursadaId = cursada.ID,
                        Alumno = cursada.CURSADA_ALUMNO.PERSONA_APELLIDO + ", " + cursada.CURSADA_ALUMNO.PERSONA_NOMBRE,
                        Documento = cursada.CURSADA_ALUMNO.PERSONA_DOCUMENTO_NUMERO,
                        Nota = nota,
                        Libre = cursada.CURSADA_ESTADO_ACADEMICO == "Libre" || cursada.CURSADA_ESTADO_ASISTENCIA == "Libre" || cursada.CURSADA_ESTADO_DEFINITIVO == "Libre",
                    });
                }
                
            }
            return Ok(alumnos);
        }
        [HttpPost]
        public IHttpActionResult PonerNotaEnParcial(int id, NotaDeParcialViewModel vm)
        {

            Cursada cursada = _db.Cursadas.Find(id);
            string nota = vm.value;
            switch (vm.name)
            {
                case ("P1"):
                    cursada.CURSADA_NOTA_P1 = nota;
                    break;
                case "R1":
                    cursada.CURSADA_NOTA_R1 = nota;
                    break;
                case "P2":
                    cursada.CURSADA_NOTA_P2 = nota;
                    break;
                case "R2":
                    cursada.CURSADA_NOTA_R2 = nota;
                    break;
            }

            _db.SaveChanges(User.Identity.Name);

            return Ok();
        }
        [HttpPost]
        public object PonerNota([FromBody]int pk, [FromBody]string value, [FromBody]string name)
        {

            Cursada cursada = _db.Cursadas.Find(pk);
            string nota = value;
            switch (name)
            {
                case ("P1"):
                    cursada.CURSADA_NOTA_P1 = nota;
                    break;
                case "R1":
                    cursada.CURSADA_NOTA_R1 = nota;
                    break;
                case "P2":
                    cursada.CURSADA_NOTA_P2 = nota;
                    break;
                case "R2":
                    cursada.CURSADA_NOTA_R2 = nota;
                    break;
            }

            _db.SaveChanges(User.Identity.Name);

            return new {};
        }

        [HttpPost]
        public IHttpActionResult SetearFecha([FromBody]FechaViewModel param)
        {
            try
            {
                Materia_x_Curso curso = _db.Materias_X_Cursos.Find(param.pk);
                switch (param.name)
                {
                    case "P1_FECHA":
                        curso.MATERIA_X_CURSO_P1_FECHA = param.value;

                        break;
                    case "R1_FECHA":
                        curso.MATERIA_X_CURSO_R1_FECHA = param.value;
                        break;
                    case "P2_FECHA":
                        curso.MATERIA_X_CURSO_P2_FECHA = param.value;
                        break;
                    case "R2_FECHA":
                        curso.MATERIA_X_CURSO_R2_FECHA = param.value;
                        break;
                }
                _db.SaveChanges(User.Identity.Name);

            }
            catch (Exception ex)
            {
                /*string json = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    {"error", ex.Message}
                });
                return Json(json, JsonRequestBehavior.AllowGet);*/
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult ConfigurarHorarios(int id, [FromBody] object Lunes)
        {
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult HabilitarSegundoParcial(int id)
        {
            try
            {
                var curso = _db.Materias_X_Cursos.Find(id);
                curso.MATERIA_X_CURSO_CANTIDAD_PARCIALES = 2;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        public object SetearTurno([FromBody]string value, [FromBody]int pk) {
            return new {};
        }

        [HttpPost]
        public IHttpActionResult CerrarNotas(int id)
        {
            try
            {
                Materia_x_Curso mc = _db.Materias_X_Cursos.Find(id);
                mc.MATERIA_X_CURSO_DEFINITIVO_EN_LIBRO = true;
                _db.SaveChanges(User.Identity.Name);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
            
        }

        [HttpGet]
        public IHttpActionResult GetAlumnosParaInscripciones(int id, string fecha)
        {
            List<object> alumnos = new List<object>();

            var FECHA = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Si ya existe una asistencia...
            if (_db.Asistencias.Any(a => a.ASISTENCIA_FECHA == FECHA && a.MATERIA_X_CURSOS_ID == id))
            {
                var asistencias = _db.Asistencias
                    .Where(a => a.MATERIA_X_CURSOS_ID == id && a.ASISTENCIA_FECHA == FECHA && a.ALUMNO != null)
                    .OrderBy(c => c.ALUMNO.PERSONA_APELLIDO)
                    .Select(a => new
                    {
                        DNI = a.ALUMNO.PERSONA_DOCUMENTO_NUMERO,
                        NOMBRE = a.ALUMNO.PERSONA_NOMBRE,
                        APELLIDO = a.ALUMNO.PERSONA_APELLIDO,
                        PRESENTE = a.ASISTENCIA_PRESENTE
                    });
                    return Ok(asistencias);
            }

            IEnumerable<Cursada> cursadas = _db.Cursadas
                .Include("CURSADA_ALUMNO")
                .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                .Where(c => c.CURSADA_MATERIAS_X_CURSOS_ID == id && c.CURSADA_ALUMNO != null).ToList();
            foreach (Cursada cursada in cursadas)
            {
                if (cursada.CURSADA_ESTADO_ASISTENCIA == "Libre")
                {
                    continue;
                }

                alumnos.Add(new
                {
                    ID = cursada.CURSADA_ALUMNOS_ID,
                    DNI = cursada.CURSADA_ALUMNO.PERSONA_DOCUMENTO_NUMERO,
                    NOMBRE = cursada.CURSADA_ALUMNO.PERSONA_NOMBRE,
                    APELLIDO = cursada.CURSADA_ALUMNO.PERSONA_APELLIDO,
                    PRESENTE = false
                });
            }

            return Ok(alumnos);
        }

        [HttpGet]
        public IHttpActionResult Info(int id, string instancia)
        {
            DateTime? fecha = new DateTime();
            try
            {
                Materia_x_Curso curso = _db.Materias_X_Cursos
                        .Include("MATERIA_X_CURSO_CARRERA")
                        .Include("MATERIA_X_CURSO_MATERIA")
                        .Include("MATERIA_X_CURSO_DOCENTE")
                        .Include("MATERIA_X_CURSO_CICLO")
                        .SingleOrDefault(m => m.ID == id);

                switch (instancia)
                {
                    case "P1":
                        fecha = curso.MATERIA_X_CURSO_P1_FECHA; break;
                    case "P2":
                        fecha = curso.MATERIA_X_CURSO_P2_FECHA; break;
                    case "R1":
                        fecha = curso.MATERIA_X_CURSO_R1_FECHA; break;
                    case "R2":
                        fecha = curso.MATERIA_X_CURSO_R2_FECHA; break;
                }
                
                var vm = new InfoCursoViewModel
                {
                    Nombre = curso.MATERIA_X_CURSO_CURSO_NOMBRE,
                    Carrera = curso.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE,
                    Materia = curso.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE,
                    Fecha = fecha != null ? fecha.Value.ToString("dd/MM/yyyy") : "",
                    Cerrado = curso.MATERIA_X_CURSO_DEFINITIVO_EN_LIBRO,
                    Docente = curso.MATERIA_X_CURSO_DOCENTE.PERSONA_APELLIDO + ", " + curso.MATERIA_X_CURSO_DOCENTE.PERSONA_NOMBRE,
                    Ciclo = curso.CICLO_ANIO
                };
                return Ok(vm);
            }
            catch (Exception e)
            {
                return NotFound();
            }
            
        }

        [HttpGet]
        public IHttpActionResult ChequearAsistencia(int? id, DateTime fecha)
        {
            if (_db.Asistencias.Any(a => a.ASISTENCIA_FECHA == fecha && a.MATERIA_X_CURSOS_ID == id))
            {
                return Ok();
            }

            return NotFound();

        }

        public class GuardarAsistenciasVM
        {
            //public int ID;
            public string DNI;
            public string NOMBRE;
            public string APELLIDO;
            public bool PRESENTE;
        }

        [HttpPost]
        public IHttpActionResult GuardarAsistencias(int? id, string FECHA, [FromBody]List<GuardarAsistenciasVM> collection)
        {

            var fecha = DateTime.ParseExact(FECHA, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            IEnumerable<Cursada> cursadas = _db.Cursadas
                .Include("CURSADA_ALUMNO")
                .OrderBy(c => c.CURSADA_ALUMNO.PERSONA_APELLIDO)
                .Where(c => c.CURSADA_MATERIAS_X_CURSOS_ID == id && c.CURSADA_ALUMNO != null).ToList();

            // SI ESTAMOS EDITANDO UNA ASISTENCIA
            if (_db.Asistencias.Any(a => a.ASISTENCIA_FECHA == fecha && a.MATERIA_X_CURSOS_ID == id))
            {
                //return BadRequest("Ya se tomó asistencia para este curso");
                var asistencias = _db.Asistencias.Include("ALUMNO").Where(a => a.ASISTENCIA_FECHA == fecha && a.MATERIA_X_CURSOS_ID == id && a.ALUMNO != null)
                                    .ToList();
                foreach (var asistencia in asistencias)
                {
                    var asist = collection.SingleOrDefault(a => a.DNI == asistencia.ALUMNO.PERSONA_DOCUMENTO_NUMERO);

                    asistencia.ASISTENCIA_PRESENTE = asist.PRESENTE;
                    
                }
                _db.SaveChanges();

                return Ok();
            }


            foreach (Cursada cursada in cursadas)
            {
                Persona alumno = cursada.CURSADA_ALUMNO;
                var asistencia = new Asistencia();
                 asistencia.ASISTENCIA_FECHA = fecha;
                asistencia.ASISTENCIA_ALUMNO_ID = cursada.CURSADA_ALUMNOS_ID;
                asistencia.MATERIA_X_CURSOS_ID = id;

                var asiste = collection.SingleOrDefault(c => c.DNI == cursada.CURSADA_ALUMNO.PERSONA_DOCUMENTO_NUMERO);
                if (asiste != null)
                {
                    asistencia.ASISTENCIA_PRESENTE = asiste.PRESENTE;
                }
                
                _db.Asistencias.Add(asistencia);
                _db.SaveChanges();
            }

            return Ok();
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