// ============================================================
// MODELO DE CONTACTO
// Representa la tabla "Contactos" en la base de datos
// Contiene las validaciones y propiedades calculadas
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManoloCRM.Models
{
    public class Contacto
    {
        // ── CLAVE PRIMARIA ────────────────────────────────────
        // Entity Framework la reconoce automáticamente como
        // clave primaria por llamarse "Id" - se autoincrementa
        public int Id { get; set; }

        // ── CÉDULA ────────────────────────────────────────────
        // Campo obligatorio, máximo 20 caracteres
        // Se valida en el servidor que no esté duplicada
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20)]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        // ── NOMBRE ────────────────────────────────────────────
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        // ── APELLIDOS ─────────────────────────────────────────
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        // ── FECHA DE NACIMIENTO ───────────────────────────────
        // Se guarda en la BD y se usa para calcular la edad
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        // ── TELÉFONO ──────────────────────────────────────────
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        // ── DIRECCIÓN ─────────────────────────────────────────
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        // ── EDAD (CALCULADA) ──────────────────────────────────
        // [NotMapped] indica que este campo NO se guarda en la BD
        // Se calcula automáticamente cada vez que se consulta
        // Algoritmo: año actual - año nacimiento, con ajuste si
        // el cumpleaños de este año todavía no ha llegado
        [NotMapped]
        [Display(Name = "Edad")]
        public int Edad
        {
            get
            {
                var hoy  = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;

                // Si aún no ha cumplido años este año, restar 1
                // Ejemplo: nació el 15-dic-1990 y hoy es mayo-2025
                // → 2025 - 1990 = 35, pero aún no cumple → devuelve 34
                if (FechaNacimiento.Date > hoy.AddYears(-edad))
                    edad--;

                return edad;
            }
        }

        // ── NOMBRE COMPLETO  ───────────────────────
        // [NotMapped] - tampoco se guarda en la BD
        // Propiedad de conveniencia para mostrar en vistas y tabla
        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
}