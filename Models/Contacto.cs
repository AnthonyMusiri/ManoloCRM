using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ManoloCRM.Models
{
public class Contacto
{
public int Id { get; set; }
    [Required(ErrorMessage = "La cédula es obligatoria")]
    [StringLength(20)]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
    [Display(Name = "Fecha de Nacimiento")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200)]
    [Display(Name = "Dirección")]
    public string Direccion { get; set; } = string.Empty;

    // [NotMapped] = este campo NO se guarda en la BD
    // La edad se calcula automáticamente desde FechaNacimiento
    [NotMapped]
    [Display(Name = "Edad")]
    public int Edad
    {
        get
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Year;
            if (FechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;
            return edad;
        }
    }

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {Apellidos}";
}
}