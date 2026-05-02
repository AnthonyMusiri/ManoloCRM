using ManoloCRM.Models;
using Microsoft.EntityFrameworkCore;
namespace ManoloCRM.Data
{
public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
{
}
    // Esto representa la tabla "Contactos" en la base de datos
    public DbSet<Contacto> Contactos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Datos de prueba que se insertan al crear la BD
        modelBuilder.Entity<Contacto>().HasData(
            new Contacto
            {
                Id = 1,
                Cedula = "1234567890",
                Nombre = "Carlos",
                Apellidos = "García López",
                FechaNacimiento = new DateTime(1990, 5, 15),
                Telefono = "300 123 4567",
                Direccion = "Calle 50 #23-10, Barranquilla"
            },
            new Contacto
            {
                Id = 2,
                Cedula = "0987654321",
                Nombre = "María",
                Apellidos = "Rodríguez Pérez",
                FechaNacimiento = new DateTime(1985, 8, 22),
                Telefono = "310 987 6543",
                Direccion = "Carrera 15 #45-30, Bogotá"
            }
        );
    }
}
}