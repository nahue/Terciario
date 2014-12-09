﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.Models;
using ColegioTerciario.Models.ViewModels;
using Newtonsoft.Json;

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
                             c.MATERIA_X_CURSO_SEDE != null ? c.MATERIA_X_CURSO_SEDE.SEDE_NOMBRE : null,
                             c.MATERIA_X_CURSO_CARRERA != null ? c.MATERIA_X_CURSO_CARRERA.CARRERA_NOMBRE : null,                       
                             c.MATERIA_X_CURSO_CURSO_NOMBRE
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

            _db.SaveChanges();

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
                _db.SaveChanges();
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