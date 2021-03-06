﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ColegioTerciario.Models.Types;
using Newtonsoft.Json;

namespace ColegioTerciario.DAL.Models
{
    [Table("Personas")]
    public class Persona : EntityBase
    {
        public int ID { get; set; }
        [Display(Name = "Codigo")]
        public string PERSONA_CODIGO { get; set; }
        [Display(Name = "Usuario")]
        public string PERSONA_USUARIO { get; set; }
        [Display(Name = "Clave")]
        public string PERSONA_CLAVE { get; set; }
        [Display(Name="Nombre")]
        [Required]
        public string PERSONA_NOMBRE { get; set; }
        [Display(Name = "Apellido")]
        [Required]
        public string PERSONA_APELLIDO { get; set; }
        [Display(Name="Nombre Para Mostrar")]
        public string PERSONA_NOMBRE_PARA_MOSTRAR { get; set; }
        [Display(Name = "Tipo de Documento")]
        public string PERSONA_DOCUMENTO_TIPO { get; set; }
        [Display(Name = "Documento Nro")]
        [Required]
        [RegularExpression("[0-9]{7,}", ErrorMessage="DNI Invalido, no puede contener espacios, puntos o simbolos")]
        public string PERSONA_DOCUMENTO_NUMERO { get; set; }
        [Display(Name = "Fecha de Nacimiento")]
        [Column(TypeName = "Date")]
        public DateTime? PERSONA_NACIMIENTO_FECHA { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "El Email no es valido")]
        public string PERSONA_EMAIL { get; set; }
        [Display(Name = "Domicilio")]
        public string PERSONA_DOMICILIO { get; set; }
        [Display(Name = "Telefono")]
        public string PERSONA_TELEFONO { get; set; }
        [Display(Name = "Sexo")]
        public string PERSONA_SEXO { get; set; }
        [Display(Name = "Fecha de Alta")]
        [Column(TypeName = "Date")]
        public DateTime? PERSONA_FECHA_ALTA { get; set; }
        [Display(Name = "Fecha de Baja")]
        [Column(TypeName = "Date")]
        public DateTime? PERSONA_FECHA_BAJA { get; set; }
        [Display(Name = "Titulo Secundario")]
        public string PERSONA_TITULO_SECUNDARIO { get; set; }
        [Display(Name = "Observacion")]
        public string PERSONA_OBSERVACION { get; set; }
        [Display(Name = "Foto")]
        public string PERSONA_FOTO { get; set; }
        [Display(Name = "CUIL")]
        public string PERSONA_CUIL { get; set; }
        [Display(Name = "Es Alumno")]   
        public bool? PERSONA_ES_ALUMNO { get; set; }
        [Display(Name = "Es Docente")]
        public bool? PERSONA_ES_DOCENTE { get; set; }
        [Display(Name = "Es NO Docente")]
        public bool? PERSONA_ES_NODOCENTE { get; set; }

        [Display(Name = "Pais de Nacimiento", Prompt = "Seleccione un Pais")]
        public int? PERSONA_NACIMIENTO_PAIS_ID { get; set; }
        [Display(Name = "Provincia de Nacimiento", Prompt = "Seleccione una Provincia")]
        public int? PERSONA_NACIMIENTO_PROVINCIA_ID { get; set; }
        [Display(Name = "Ciudad de Nacimiento",Prompt="Seleccione una Ciudad")]
        public int? PERSONA_NACIMIENTO_CIUDAD_ID { get; set; }
        [Display(Name = "Barrio", Prompt = "Seleccione un Barrio")]
        public int? PERSONA_NACIMIENTO_BARRIO_ID { get; set; }

        [ForeignKey("PERSONA_NACIMIENTO_PAIS_ID")]
        public virtual Pais PERSONA_NACIMIENTO_PAIS { get; set; }
        [ForeignKey("PERSONA_NACIMIENTO_PROVINCIA_ID")]
        public virtual Provincia PERSONA_NACIMIENTO_PROVINCIA { get; set; }
        [ForeignKey("PERSONA_NACIMIENTO_CIUDAD_ID")]
        public virtual Ciudad PERSONA_NACIMIENTO_CIUDAD { get; set; }
        [ForeignKey("PERSONA_NACIMIENTO_BARRIO_ID")]
        public virtual Barrio PERSONA_BARRIO { get; set; }

        [JsonIgnore]
        public virtual ICollection<Acta_Examen> ACTAS_PRECIDIDAS { get; set; }
        [JsonIgnore]
        public virtual ICollection<Acta_Examen> ACTAS_VOCAL1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<Acta_Examen> ACTAS_VOCAL2 { get; set; }
        [JsonIgnore]
        public virtual ICollection<Acta_Examen_Detalle> ACTAS_EXAMENES_DETALLES { get; set; }
        [JsonIgnore]
        public virtual ICollection<Cursada> PERSONA_CURSADAS { get; set; } 
        #region Metodos

        public string PERSONA_NOMBRE_COMPLETO {
            get {
                return PERSONA_APELLIDO + ", " + PERSONA_NOMBRE;
            }
        }
        #endregion
    }
}
