using ManoloCRM.Data;
using ManoloCRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace ManoloCRM.Controllers
{
public class ContactosController : Controller
{
private readonly AppDbContext _context;
    public ContactosController(AppDbContext context)
    {
        _context = context;
    }

    private bool EstaLogueado() =>
        HttpContext.Session.GetString("UsuarioLogueado") != null;

    // ── DASHBOARD ──────────────────────────────────────────
    public async Task<IActionResult> Dashboard()
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");

        var contactos = await _context.Contactos.ToListAsync();

        if (!contactos.Any())
        {
            ViewBag.TotalContactos = 0;
            ViewBag.EdadPromedio = 0;
            ViewBag.MasJoven = null;
            ViewBag.MayorEdad = null;
            ViewBag.RangoEdades = new Dictionary<string, int>();
            ViewBag.Recientes = new List<Contacto>();
            return View();
        }

        var edades = contactos.Select(c => c.Edad).ToList();

        ViewBag.TotalContactos = contactos.Count;
        ViewBag.EdadPromedio = Math.Round(edades.Average(), 1);
        ViewBag.MasJoven = contactos.OrderByDescending(c => c.FechaNacimiento).First();
        ViewBag.MayorEdad = contactos.OrderBy(c => c.FechaNacimiento).First();

        ViewBag.RangoEdades = new Dictionary<string, int>
        {
            { "0-17",  contactos.Count(c => c.Edad <= 17) },
            { "18-30", contactos.Count(c => c.Edad >= 18 && c.Edad <= 30) },
            { "31-45", contactos.Count(c => c.Edad >= 31 && c.Edad <= 45) },
            { "46-60", contactos.Count(c => c.Edad >= 46 && c.Edad <= 60) },
            { "60+",   contactos.Count(c => c.Edad > 60) }
        };

        ViewBag.Recientes = contactos.TakeLast(5).ToList();
        return View();
    }

    // ── ÍNDICE / LISTA ──────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");

        return View(await _context.Contactos.ToListAsync());
    }

    // ── CREAR ───────────────────────────────────────────────
    public IActionResult Crear()
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Contacto contacto)
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");

        if (await _context.Contactos.AnyAsync(c => c.Cedula == contacto.Cedula))
            ModelState.AddModelError("Cedula", "Ya existe un contacto con esa cédula.");

        if (ModelState.IsValid)
        {
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Contacto registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(contacto);
    }

    // ── EDITAR ──────────────────────────────────────────────
    public async Task<IActionResult> Editar(int? id)
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");

        if (id == null) return NotFound();
        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto == null) return NotFound();
        return View(contacto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Contacto contacto)
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");

        if (id != contacto.Id) return NotFound();

        if (await _context.Contactos.AnyAsync(c => c.Cedula == contacto.Cedula && c.Id != id))
            ModelState.AddModelError("Cedula", "Ya existe otro contacto con esa cédula.");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(contacto);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Contacto actualizado exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Contactos.Any(c => c.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(contacto);
    }

    // ── ELIMINAR (solo POST, el modal está en Index) ────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        if (!EstaLogueado())
            return RedirectToAction("Login", "Account");

        var contacto = await _context.Contactos.FindAsync(id);
        if (contacto != null)
        {
            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();
            TempData["Exito"] = $"Contacto {contacto.NombreCompleto} eliminado.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ── EXPORTAR CSV ────────────────────────────────────────
    public async Task<IActionResult> ExportarCsv()
    {
    if (!EstaLogueado())
        return RedirectToAction("Login", "Account");

    var contactos = await _context.Contactos.ToListAsync();
    var sb = new StringBuilder();

    // Punto y coma para Excel
    sb.AppendLine("Cedula;Nombre;Apellidos;Fecha Nacimiento;Edad;Telefono;Direccion");
    foreach (var c in contactos)
        sb.AppendLine($"{c.Cedula};{c.Nombre};{c.Apellidos};" +
                      $"{c.FechaNacimiento:yyyy-MM-dd};{c.Edad};" +
                      $"{c.Telefono};\"{c.Direccion}\"");

    // UTF-8 con BOM para que Excel muestre tildes y ñ correctamente
    var bytes = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                .ToArray();

    var fecha = DateTime.Now.ToString("yyyy-MM-dd");
    return File(bytes, "text/csv", $"contactos_manolo_{fecha}.csv");
    }
}
}