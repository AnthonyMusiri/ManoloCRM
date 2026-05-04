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
        public int Id { get; set; }

        // ── CÉDULA ────────────────────────────────────────────
        // Solo números, mínimo 6 y máximo 20 dígitos
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20, MinimumLength = 6,
            ErrorMessage = "La cédula debe tener entre 6 y 20 dígitos")]
        [RegularExpression(@"^\d+$",
            ErrorMessage = "La cédula solo puede contener números")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        // ── NOMBRE ────────────────────────────────────────────
        // Solo letras y espacios, sin números ni caracteres especiales
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
            ErrorMessage = "El nombre solo puede contener letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        // ── APELLIDOS ─────────────────────────────────────────
        // Solo letras y espacios, sin números ni caracteres especiales
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$",
            ErrorMessage = "Los apellidos solo pueden contener letras")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        // ── FECHA DE NACIMIENTO ───────────────────────────────
        // Debe ser una fecha pasada, no futura ni absurda
        // Mínimo: año 1900 | Máximo: hoy
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        // ── TELÉFONO ──────────────────────────────────────────
        // Solo números, espacios y guiones, mínimo 7 dígitos
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20, MinimumLength = 7,
            ErrorMessage = "El teléfono debe tener mínimo 7 dígitos")]
        [RegularExpression(@"^[\d\s\-\+]+$",
            ErrorMessage = "El teléfono solo puede contener números")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        // ── DIRECCIÓN ─────────────────────────────────────────
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        // ── EDAD ──────────────────────────────────
        // [NotMapped] = NO se guarda en la BD
        // Se calcula desde FechaNacimiento cada vez que se consulta
        [NotMapped]
        [Display(Name = "Edad")]
        public int Edad
        {
            get
            {
                var hoy  = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad))
                    edad--;
                return edad;
            }
        }

        // ── NOMBRE COMPLETO ───────────────────────
        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
}