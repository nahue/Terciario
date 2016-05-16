using System.Globalization;
using ColegioTerciario.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ColegioTerciario.Models.ViewModels;

namespace ColegioTerciario.Models.Repositories
{
    public class PersonasRepository : IPersonasRepository
    {
        private readonly ColegioTerciarioContext _dbContext = new ColegioTerciarioContext();
        private bool _disposed = false;

        public IEnumerable<Persona> GetPersonasByActa(int acta_examen_id)
        {
            var alumnos = new List<Persona>();
            
            var actasDetalles = _dbContext.Actas_Examenes_Detalles
               .Include("ACTA_EXAMEN_DETALLE_ALUMNO")
               .Where(c => c.ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID == acta_examen_id)
               .OrderBy(a => a.ACTA_EXAMEN_DETALLE_ALUMNO.PERSONA_APELLIDO)
               .ToList();

            foreach (Acta_Examen_Detalle detalle in actasDetalles)
            {
                alumnos.Add(detalle.ACTA_EXAMEN_DETALLE_ALUMNO);
            }
            return alumnos;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._dbContext.Dispose();
                }
            }
            this._disposed = true;
        }

        public IEnumerable<Persona> GetAlumnos()
        {
            return _dbContext.Personas.OrderBy(a => a.PERSONA_APELLIDO).ToList();
        }


        public IQueryable<SituacionAcademicaPorCiclosViewModel> GetSituacionAcademicaPorCiclos(Persona persona)
        {
            var cursadas = from c in _dbContext.Cursadas
                           where c.CURSADA_ALUMNOS_ID == persona.ID
                           group c by c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE into grp
                           select new SituacionAcademicaPorCiclosViewModel()
                           {
                               Carrera = grp.Key,
                               Cursadas = grp.GroupBy(m => m.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CICLO).Select(g => new SituacionAcademicaCursadasViewModel()
                               {
                                   
                                   Ciclo = g.Key.CICLO_NOMBRE,
                                   Materias = g.Select(c => new SituacionAcademicaMateriasViewModel()
                                   {
                                       MateriaXCursoID = c.CURSADA_MATERIA_X_CURSO.ID,
                                       MateriaNombre = c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_MATERIA.MATERIA_NOMBRE,
                                       P1 = c.CURSADA_NOTA_P1,
                                       R1 = c.CURSADA_NOTA_R1,
                                       P2 = c.CURSADA_NOTA_P2,
                                       R2 = c.CURSADA_NOTA_R2,
                                       P1Fecha = c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_P1_FECHA,
                                       R1Fecha = c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_R1_FECHA,
                                       P2Fecha = c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_P2_FECHA,
                                       R2Fecha = c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_R2_FECHA
                                   }
                                   )
                               })
                           };
            return cursadas;
        }

        public IQueryable<SituacionAcademicaPorMateriasViewModel> GetSituacionAcademicaPorMaterias(Persona persona)
        {
            var cursadas = from c in _dbContext.Cursadas
                            where c.CURSADA_ALUMNOS_ID == persona.ID
                            group c by c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE into grp
                            select new SituacionAcademicaPorMateriasViewModel()
                            {
                                Carrera = grp.Key,
                                Materias = grp.GroupBy(m => m.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_MATERIA).Select(c => new SituacionAcademicaPorMateriasMateriasViewModel()
                                {
                                    Materia = c.Key.MATERIA_NOMBRE,
                                    Cursadas = c.Select(cursada => new SituacionAcademicaMateriasViewModel()
                                    {
                                        MateriaXCursoID = cursada.CURSADA_MATERIA_X_CURSO.ID,
                                        MateriaNombre  = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CICLO.CICLO_NOMBRE,
                                        P1 = cursada.CURSADA_NOTA_P1,
                                        R1 = cursada.CURSADA_NOTA_R1,
                                        P2 = cursada.CURSADA_NOTA_P2,
                                        R2 = cursada.CURSADA_NOTA_R2,
                                        P1Fecha = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_P1_FECHA,
                                        R1Fecha = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_R1_FECHA,
                                        P2Fecha = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_P2_FECHA,
                                        R2Fecha = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_R2_FECHA
                                    })
                                })
                            };

            return cursadas;
        }

        public IQueryable<SituacionFinalesViewModel> GetFinales(Persona persona)
        {
            var actas = from a in _dbContext.Actas_Examenes_Detalles
                        where (a.ACTA_EXAMEN_DETALLE_ALUMNOS_ID == persona.ID && a.DeletedAt == null)
                        group a by a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_CARRERA into carreras
                        select new SituacionFinalesViewModel
                    {
                        Persona = persona.PERSONA_NOMBRE_COMPLETO,
                        Carrera = carreras.Key.CARRERA_NOMBRE,
                        CarreraID = carreras.Key.ID,
                        Finales = carreras.GroupBy(c => c.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_ANIO).Select(a => new FinalesViewModel
                        {
                            Anio = a.Key == "1" ? "2012" : a.Key == "2" ? "2013" : a.Key == "3" ? "2014" : "",
                            Examenes = a.Select(f => new ExamenesFinalesViewModel
                            {
                                ActaId = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ID,
                                Anio = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_ANIO,
                                Fecha = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FECHA,
                                CodigoMateria = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_CODIGO,
                                Materia = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_NOMBRE,
                                Nota = f.ACTA_EXAMEN_DETALLE_NOTA,
                                Estado = f.ACTA_EXAMEN_DETALLE_ESTADO
                            }).OrderBy(f => new { f.CodigoMateria, f.Fecha })
                        })
                    };

            return actas;
        }

        public List<Equivalencia_Detalle> GetEquivalencias(Persona persona)
        {
            try
            {
                var equivalencias = _dbContext.Equivalencias.SingleOrDefault(e => e.EQUIVALENCIA_ALUMNO_ID == persona.ID).EQUIVALENCIA_DETALLES.ToList();
                return equivalencias;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        private string CalcularCiclo(string anio)
        {
            return (int.Parse(anio, NumberStyles.Number) + 2011).ToString();
        }

        public List<ListaAnaliticoViewModel> GetAnalitico(Persona persona, int? carreraID)
        {
            var actas2 = /*_dbContext.Actas_Examenes_Detalles.Where(a => 
                    a.ACTA_EXAMEN_DETALLE_ALUMNOS_ID == persona.ID && a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_CARRERAS_ID == carreraID).
                    GroupBy(a => a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_ANIO).Select(
                        a => new ListaAnaliticoViewModel {
                            Anio = a.Key,
                            Finales = a.OrderBy(b => b.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_CODIGO)
                                        .Select(b => new AnaliticoFinalesViewModel
                                        {
                                            ActaId = b.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ID
                                        })
                        }
                    );*/
                
                from a in _dbContext.Actas_Examenes_Detalles
                         where a.ACTA_EXAMEN_DETALLE_ALUMNOS_ID == persona.ID
                            && a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_CARRERAS_ID == carreraID
                         group a by a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_ANIO into anios

                         select new ListaAnaliticoViewModel
                         {
                             Anio = anios.Key,
                             Finales = from anio in anios
                                       orderby new
                                       {
                                           anio.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_CODIGO,
                                           anio.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FECHA
                                       }

                                       group anio by anio.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_CODIGO into finales
                                       let final = finales.OrderByDescending(b => b.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FECHA).FirstOrDefault()
                                       select new AnaliticoFinalesViewModel
                                       {
                                           ActaId =  final.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ID,
                                           Fecha = final.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FECHA,
                                           Materia = final.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_NOMBRE,
                                           Nota = final.ACTA_EXAMEN_DETALLE_NOTA,
                                           Estado = final.ACTA_EXAMEN_DETALLE_ESTADO,
                                           Libro = final.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_LIBRO.ToUpper(),
                                           Folio = final.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FOLIO.ToUpper()
                                       }

                         };
            
            return actas2.ToList();
            var actas = from a in _dbContext.Actas_Examenes_Detalles
                        where a.ACTA_EXAMEN_DETALLE_ALUMNOS_ID == persona.ID
                         && a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_CARRERA.ID == carreraID
 //group a by a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_CARRERA into carreras
                        select new SituacionFinalesViewModel
                        {
                            Persona = persona.PERSONA_NOMBRE_COMPLETO,
                            Carrera = a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_CARRERA.CARRERA_NOMBRE,
                            Finales = a.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTAS_EXAMENES_DETALLES.GroupBy(c => c.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_ANIO).Select(b => new FinalesViewModel
                            {
                                Anio = b.Key == "1" ? "2012" : b.Key == "2" ? "2013" : b.Key == "3" ? "2014" : "",
                                Examenes = b.Select(f => new ExamenesFinalesViewModel
                                {
                                    ActaId = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ID,
                                    Anio = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_ANIO,
                                    Fecha = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FECHA,
                                    Materia = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_MATERIA.MATERIA_NOMBRE,
                                    Nota = f.ACTA_EXAMEN_DETALLE_NOTA,
                                    Estado = f.ACTA_EXAMEN_DETALLE_ESTADO,
                                    Libro = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_LIBRO,
                                    Folio = f.ACTA_EXAMEN_DETALLE_ACTA_EXAMEN.ACTA_EXAMEN_FOLIO
                                })
                            })
                        };
            //return actas.First();
        }

        public bool EsAlumnoRegular(Persona persona)
        {
            var corriente = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            var ciclo = _dbContext.Ciclos.SingleOrDefault(c => c.CICLO_ANIO == corriente);
            var cursadas = _dbContext.Cursadas
                .Include("CURSADA_MATERIA_X_CURSO")
                .Include("CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_MATERIA")
                .Where(c => c.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_CICLOS_ID == ciclo.ID && c.CURSADA_ALUMNOS_ID == persona.ID)
                .ToList();

            foreach (Cursada cursada in cursadas)
            {
                var materia = cursada.CURSADA_MATERIA_X_CURSO.MATERIA_X_CURSO_MATERIA;

                if (materia.MATERIA_DURACION == Materia.Anual)
                {
                    return true;
                }
                else if (materia.MATERIA_DURACION == Materia.PrimerCuatrimestre)
                {
                    return false;
                }
                else if (materia.MATERIA_DURACION == Materia.SegundoCuatrimestre)
                {
                    return false;
                }
                
            }

            return false;
        }
    }
}