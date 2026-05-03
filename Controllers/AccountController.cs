// ============================================================
// CONTROLADOR DE AUTENTICACIÓN
// Maneja el login y logout del administrador del sistema
// ============================================================

using ManoloCRM.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManoloCRM.Controllers
{
    public class AccountController : Controller
    {
        // ── CREDENCIALES ─────────────────────────────────────
        // En producción estas irían cifradas en la base de datos
        // usando ASP.NET Core Identity + BCrypt
        private const string ADMIN_USUARIO  = "admin";
        private const string ADMIN_PASSWORD = "admin123";

        // ── LOGIN GET ─────────────────────────────────────────
        // Muestra el formulario de login
        // Si ya hay sesión activa, redirige directo al Dashboard
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UsuarioLogueado") != null)
                return RedirectToAction("Dashboard", "Contactos");

            return View();
        }

        // ── LOGIN POST ────────────────────────────────────────
        // Recibe las credenciales del formulario y las valida
        // Si son correctas, crea la sesión y redirige al Dashboard
        // Si no, muestra un mensaje de error en el formulario
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección contra ataques CSRF
        public IActionResult Login(LoginViewModel model)
        {
            // Verificar que el formulario pasó las validaciones básicas
            if (!ModelState.IsValid)
                return View(model);

            // Comparar credenciales ingresadas con las del sistema
            if (model.Usuario == ADMIN_USUARIO && model.Password == ADMIN_PASSWORD)
            {
                // Guardar usuario en sesión para mantenerlo autenticado
                HttpContext.Session.SetString("UsuarioLogueado", model.Usuario);
                return RedirectToAction("Dashboard", "Contactos");
            }

            // Credenciales incorrectas: agregar error al formulario
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // ── LOGOUT ───────────────────────────────────────────
        // Limpia toda la sesión y regresa al login
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}