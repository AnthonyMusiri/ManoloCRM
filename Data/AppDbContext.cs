// ============================================================
// CONTEXTO DE BASE DE DATOS
// Clase principal de Entity Framework Core
// Define las tablas y configura los datos iniciales (seed data)
// ============================================================

using ManoloCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace ManoloCRM.Data
{
    public class AppDbContext : DbContext
    {
        // ── CONSTRUCTOR ───────────────────────────────────────
        // Recibe la configuración inyectada desde Program.cs
        // (cadena de conexión, tipo de BD, etc.)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ── TABLAS DE LA BASE DE DATOS ────────────────────────
        // Cada DbSet<T> representa una tabla en la BD
        // Entity Framework genera el SQL automáticamente
        public DbSet<Contacto> Contactos { get; set; }

        // ── CONFIGURACIÓN DEL MODELO ──────────────────────────
        // Se ejecuta una sola vez al crear/migrar la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── SEED DATA ─────────────────────────────────────
            // Contactos de prueba que se insertan automáticamente
            // al ejecutar "dotnet ef database update"
            // Útiles para tener datos al hacer la demo
            modelBuilder.Entity<Contacto>().HasData(
                new Contacto
                {
                    Id             = 1,
                    Cedula         = "1234567890",
                    Nombre         = "Carlos",
                    Apellidos      = "García López",
                    FechaNacimiento = new DateTime(1990, 5, 15),
                    Telefono       = "300 123 4567",
                    Direccion      = "Calle 50 #23-10, Barranquilla"
                },
                new Contacto
                {
                    Id             = 2,
                    Cedula         = "0987654321",
                    Nombre         = "María",
                    Apellidos      = "Rodríguez Pérez",
                    FechaNacimiento = new DateTime(1985, 8, 22),
                    Telefono       = "310 987 6543",
                    Direccion      = "Carrera 15 #45-30, Bogotá"
                }
            );
        }
    }
}