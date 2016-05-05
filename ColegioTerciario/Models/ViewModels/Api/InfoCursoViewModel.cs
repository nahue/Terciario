using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ColegioTerciario.Models.ViewModels.Api
{
    public class InfoCursoViewModel
    {
        public string Nombre { get; set; }
        public string Carrera { get; set; }
        public string Materia { get; set; }
        public string Fecha { get; set; }

        public bool Cerrado { get; set; }

        public string Ciclo { get; set; }

        public string Docente { get; set; }
    }
}