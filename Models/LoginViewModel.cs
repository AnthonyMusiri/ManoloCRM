// ============================================================
// MODELO DEL FORMULARIO DE LOGIN
// No representa una tabla en la BD - es solo para el formulario
// Contiene las validaciones de los campos de autenticación
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ManoloCRM.Models
{
    public class LoginViewModel
    {
        // ── USUARIO ───────────────────────────────────────────
        // Campo obligatorio que se compara con ADMIN_USUARIO
        // en AccountController
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = string.Empty;

        // ── CONTRASEÑA ────────────────────────────────────────
        // DataType.Password hace que el input se muestre
        // como ******* en el navegador automáticamente
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
    }
}