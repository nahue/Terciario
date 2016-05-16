using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using ColegioTerciario.DAL.Models;
using ColegioTerciario.DAL.Models.Inscripciones;

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using ColegioTerciario.Models.User;
using TrackerEnabledDbContext.Identity;

namespace ColegioTerciario.Models
{

    public partial class ColegioTerciarioContext : TrackerIdentityContext<ApplicationUser>
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ColegioTerciarioContext() : base("name=ColegioTerciarioContext")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Barrio> Barrios { get; set; }
        public DbSet<Ciclo> Ciclos { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Acta_Examen> Actas_Examenes { get; set; }
        public DbSet<Acta_Examen_Detalle> Actas_Examenes_Detalles { get; set; }
        public DbSet<Cursada> Cursadas { get; set; }
        public DbSet<Hora> Horas { get; set; }
        public DbSet<Horario_Cursada> Horarios_Cursadas { get; set; }
        public DbSet<Materia_x_Curso> Materias_X_Cursos { get; set; }
        public DbSet<Turno_Examen> Turnos_Examenes { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Equivalencia> Equivalencias { get; set; }
        public DbSet<Equivalencia_Detalle> Equivalencias_Detalles { get; set; }
        public DbSet<Inscripciones> Inscripciones { get; set; }
        public DbSet<InscripcionesConfig> Inscripciones_Config { get; set; }
        public DbSet<InscripcionesCarrera> Inscripciones_Carrera { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new EntityBaseConfiguration());
            modelBuilder.Entity<Acta_Examen>()
                .HasMany(a => a.ACTAS_EXAMENES_DETALLES)
                .WithOptional()
                .HasForeignKey(a => a.ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Acta_Examen>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Acta_Examen_Detalle>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Persona>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Ciudad>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Provincia>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Pais>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Barrio>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Ciclo>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Carrera>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Materia>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Materia_x_Curso>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Turno_Examen>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Cursada>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Sede>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Equivalencia>()
            .Map(m => m.Requires("IsDeleted").HasValue(false))
            .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Equivalencia_Detalle>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Inscripciones>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<InscripcionesCarrera>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<InscripcionesConfig>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            modelBuilder.Entity<Asistencia>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
            base.OnModelCreating(modelBuilder);
        }


        public static ColegioTerciarioContext Create()
        {
            return new ColegioTerciarioContext();
        }

        public override int SaveChanges()
        {
            var changedEntities = ChangeTracker.Entries();

            foreach (var changedEntity in changedEntities)
            {/*
                if (changedEntity.Entity is Entity)
                {
                    var entity = (Entity)changedEntity.Entity;

                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.OnBeforeInsert();
                            break;

                        case EntityState.Modified:
                            entity.OnBeforeUpdate();
                            break;

                    }
                }*/
                
                if (changedEntity.State == EntityState.Deleted)
                {
                    SoftDelete(changedEntity);
                }
            }

            return base.SaveChanges();
        }

        private void SoftDelete(DbEntityEntry entry)
        {
            Type entryEntityType = entry.Entity.GetType();

            string tableName = GetTableName(entryEntityType);
            string primaryKeyName = GetPrimaryKeyName(entryEntityType);

            string sql =
                string.Format(
                    "UPDATE {0} SET IsDeleted = 1, DeletedAt = GETDATE() WHERE {1} = @id",
                        tableName, primaryKeyName);

            Database.ExecuteSqlCommand(
                sql,
                new SqlParameter("@id", entry.OriginalValues[primaryKeyName]));

            var hardDelete = false;
            switch (tableName)
            {
                case "[dbo].[Actas_Examenes]":
                    Database.ExecuteSqlCommand(
                        "UPDATE dbo.Actas_Examenes_Detalles SET IsDeleted = 1, DeletedAt = GETDATE() WHERE ACTA_EXAMEN_DETALLE_ACTAS_EXAMENES_ID = @id",
                        new SqlParameter("@id", entry.OriginalValues[primaryKeyName]));
                    break;

                /**
                 * Los modelos de ASP Identity no manejan el concepto de Soft Delete asique obviamos el proceso para
                 * estas tablas.
                 */
                case "[dbo].[AspNetRoles]":
                case "[dbo].[AspNetUserClaims]":
                case "[dbo].[AspNetUserLogins]":
                case "[dbo].[AspNetUserRoles]":
                case "[dbo].[AspNetUsers]":
                    hardDelete = true;
                    break;

            }

            if (hardDelete != true)
            {
                // prevent hard delete            
                entry.State = EntityState.Detached;
            }
        }
        
    }

    public partial class ColegioTerciarioContext
    {
        private static Dictionary<Type, EntitySetBase> _mappingCache =
            new Dictionary<Type, EntitySetBase>();

        private string GetTableName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);

            return string.Format("[{0}].[{1}]",
                es.MetadataProperties["Schema"].Value,
                es.MetadataProperties["Table"].Value);
        }

        private string GetPrimaryKeyName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);

            return es.ElementType.KeyMembers[0].Name;
        }

        private EntitySetBase GetEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;

                string typeName = ObjectContext.GetObjectType(type).Name;

                var es = octx.MetadataWorkspace
                                .GetItemCollection(DataSpace.SSpace)
                                .GetItems<EntityContainer>()
                                .SelectMany(c => c.BaseEntitySets
                                                .Where(e => e.Name == typeName))
                                .FirstOrDefault();

                if (es == null)
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }
    }

    
}
