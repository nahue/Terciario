﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ColegioTerciario.Models
{
    public class ColegioTerciarioContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ColegioTerciarioContext() : base("name=ColegioTerciarioContext")
        {
        }

        public System.Data.Entity.DbSet<ColegioTerciario.DAL.Models.Persona> Personas { get; set; }
        public System.Data.Entity.DbSet<ColegioTerciario.DAL.Models.Pais> Paises { get; set; }
        public System.Data.Entity.DbSet<ColegioTerciario.DAL.Models.Ciudad> Ciudades { get; set; }

        public System.Data.Entity.DbSet<ColegioTerciario.DAL.Models.Provincia> Provincias { get; set; }
    }
}
