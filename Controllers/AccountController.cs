using ManoloCRM.Models;
using Microsoft.AspNetCore.Mvc;
namespace ManoloCRM.Controllers
{
public class AccountController : Controller
{
// Credenciales hardcodeadas (en producción irían en la Base de datos cifradas)
private const string ADMIN_USUARIO = "admin";
private const string ADMIN_PASSWORD = "admin123";
    [HttpGet]
    public IActionResult Login()
    {
        // Si ya esta logeado, ir directo al Dashboard
        if (HttpContext.Session.GetString("UsuarioLogueado") != null)
            return RedirectToAction("Dashboard", "Contactos");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Usuario == ADMIN_USUARIO && model.Password == ADMIN_PASSWORD)
        {
            HttpContext.Session.SetString("UsuarioLogueado", model.Usuario);
            return RedirectToAction("Dashboard", "Contactos");
        }

        ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
}