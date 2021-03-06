namespace ColegioTerciario.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntityBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actas_Examenes", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Actas_Examenes", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Actas_Examenes", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Actas_Examenes", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Carreras", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Carreras", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Carreras", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Carreras", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Materias", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Materias", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Materias", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Materias", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Personas", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Personas", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Personas", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Personas", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Actas_Examenes_Detalles", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Actas_Examenes_Detalles", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Actas_Examenes_Detalles", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Actas_Examenes_Detalles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Barrios", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Barrios", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Barrios", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Barrios", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Ciudades", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Ciudades", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ciudades", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ciudades", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cursadas", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Cursadas", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Cursadas", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Cursadas", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Materias_X_Cursos", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Materias_X_Cursos", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Materias_X_Cursos", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Materias_X_Cursos", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Horarios_Cursadas", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Horarios_Cursadas", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Horarios_Cursadas", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Horarios_Cursadas", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Horas", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Horas", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Horas", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Horas", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ciclos", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Ciclos", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ciclos", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Ciclos", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Turnos_Examenes", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Turnos_Examenes", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Turnos_Examenes", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Turnos_Examenes", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sedes", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Sedes", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sedes", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sedes", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Paises", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Paises", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Paises", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Paises", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Provincias", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Provincias", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Provincias", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Provincias", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Matriculas", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Matriculas", "DeletedAt", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Matriculas", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Matriculas", "ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matriculas", "ModifiedDate");
            DropColumn("dbo.Matriculas", "CreatedDate");
            DropColumn("dbo.Matriculas", "DeletedAt");
            DropColumn("dbo.Matriculas", "IsDeleted");
            DropColumn("dbo.Provincias", "IsDeleted");
            DropColumn("dbo.Provincias", "ModifiedDate");
            DropColumn("dbo.Provincias", "CreatedDate");
            DropColumn("dbo.Provincias", "DeletedAt");
            DropColumn("dbo.Paises", "IsDeleted");
            DropColumn("dbo.Paises", "ModifiedDate");
            DropColumn("dbo.Paises", "CreatedDate");
            DropColumn("dbo.Paises", "DeletedAt");
            DropColumn("dbo.Sedes", "IsDeleted");
            DropColumn("dbo.Sedes", "ModifiedDate");
            DropColumn("dbo.Sedes", "CreatedDate");
            DropColumn("dbo.Sedes", "DeletedAt");
            DropColumn("dbo.Turnos_Examenes", "IsDeleted");
            DropColumn("dbo.Turnos_Examenes", "ModifiedDate");
            DropColumn("dbo.Turnos_Examenes", "CreatedDate");
            DropColumn("dbo.Turnos_Examenes", "DeletedAt");
            DropColumn("dbo.Ciclos", "IsDeleted");
            DropColumn("dbo.Ciclos", "ModifiedDate");
            DropColumn("dbo.Ciclos", "CreatedDate");
            DropColumn("dbo.Ciclos", "DeletedAt");
            DropColumn("dbo.Horas", "ModifiedDate");
            DropColumn("dbo.Horas", "CreatedDate");
            DropColumn("dbo.Horas", "DeletedAt");
            DropColumn("dbo.Horas", "IsDeleted");
            DropColumn("dbo.Horarios_Cursadas", "ModifiedDate");
            DropColumn("dbo.Horarios_Cursadas", "CreatedDate");
            DropColumn("dbo.Horarios_Cursadas", "DeletedAt");
            DropColumn("dbo.Horarios_Cursadas", "IsDeleted");
            DropColumn("dbo.Materias_X_Cursos", "IsDeleted");
            DropColumn("dbo.Materias_X_Cursos", "ModifiedDate");
            DropColumn("dbo.Materias_X_Cursos", "CreatedDate");
            DropColumn("dbo.Materias_X_Cursos", "DeletedAt");
            DropColumn("dbo.Cursadas", "IsDeleted");
            DropColumn("dbo.Cursadas", "ModifiedDate");
            DropColumn("dbo.Cursadas", "CreatedDate");
            DropColumn("dbo.Cursadas", "DeletedAt");
            DropColumn("dbo.Ciudades", "IsDeleted");
            DropColumn("dbo.Ciudades", "ModifiedDate");
            DropColumn("dbo.Ciudades", "CreatedDate");
            DropColumn("dbo.Ciudades", "DeletedAt");
            DropColumn("dbo.Barrios", "IsDeleted");
            DropColumn("dbo.Barrios", "ModifiedDate");
            DropColumn("dbo.Barrios", "CreatedDate");
            DropColumn("dbo.Barrios", "DeletedAt");
            DropColumn("dbo.Actas_Examenes_Detalles", "IsDeleted");
            DropColumn("dbo.Actas_Examenes_Detalles", "ModifiedDate");
            DropColumn("dbo.Actas_Examenes_Detalles", "CreatedDate");
            DropColumn("dbo.Actas_Examenes_Detalles", "DeletedAt");
            DropColumn("dbo.Personas", "IsDeleted");
            DropColumn("dbo.Personas", "ModifiedDate");
            DropColumn("dbo.Personas", "CreatedDate");
            DropColumn("dbo.Personas", "DeletedAt");
            DropColumn("dbo.Materias", "IsDeleted");
            DropColumn("dbo.Materias", "ModifiedDate");
            DropColumn("dbo.Materias", "CreatedDate");
            DropColumn("dbo.Materias", "DeletedAt");
            DropColumn("dbo.Carreras", "IsDeleted");
            DropColumn("dbo.Carreras", "ModifiedDate");
            DropColumn("dbo.Carreras", "CreatedDate");
            DropColumn("dbo.Carreras", "DeletedAt");
            DropColumn("dbo.Actas_Examenes", "IsDeleted");
            DropColumn("dbo.Actas_Examenes", "ModifiedDate");
            DropColumn("dbo.Actas_Examenes", "CreatedDate");
            DropColumn("dbo.Actas_Examenes", "DeletedAt");
        }
    }
}
