using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ManoloCRM.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contactos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cedula = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Contactos",
                columns: new[] { "Id", "Apellidos", "Cedula", "Direccion", "FechaNacimiento", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "García López", "1234567890", "Calle 50 #23-10, Barranquilla", new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carlos", "300 123 4567" },
                    { 2, "Rodríguez Pérez", "0987654321", "Carrera 15 #45-30, Bogotá", new DateTime(1985, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "María", "310 987 6543" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contactos");
        }
    }
}
