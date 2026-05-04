// ============================================================
// CONTROLADOR PRINCIPAL DE CONTACTOS
// Maneja el CRUD completo + Dashboard + Exportar CSV
// Todas las acciones verifican sesión activa antes de ejecutar
// ============================================================

using ManoloCRM.Data;
using ManoloCRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ManoloCRM.Controllers
{
    public class ContactosController : Controller
    {
        // ── INYECCIÓN DE DEPENDENCIAS ─────────────────────────
        // El DbContext se inyecta automáticamente por ASP.NET Core
        // Permite acceder a la base de datos en todas las acciones
        private readonly AppDbContext _context;

        public ContactosController(AppDbContext context)
        {
            _context = context;
        }

        // ── MÉTODO AUXILIAR DE AUTENTICACIÓN ─────────────────
        // Verifica si existe una sesión activa
        // Se llama al inicio de cada acción para proteger las rutas
        private bool EstaLogueado() =>
            HttpContext.Session.GetString("UsuarioLogueado") != null;

        // ── MÉTODO AUXILIAR DE VALIDACIÓN DE FECHA ───────────
        // Valida que la fecha de nacimiento sea razonable
        // Se reutiliza en Crear y Editar para no repetir código
        private void ValidarFecha(DateTime fecha)
        {
            if (fecha > DateTime.Today)
                ModelState.AddModelError("FechaNacimiento",
                    "La fecha de nacimiento no puede ser una fecha futura.");

            if (fecha.Year < 1900)
                ModelState.AddModelError("FechaNacimiento",
                    "Ingresa una fecha de nacimiento válida.");

            if (fecha < DateTime.Today.AddYears(-120))
                ModelState.AddModelError("FechaNacimiento",
                    "Ingresa una fecha de nacimiento válida.");
        }


        // ════════════════════════════════════════════════════════
        // DASHBOARD
        // Calcula y muestra estadísticas generales del sistema
        // ════════════════════════════════════════════════════════

        public async Task<IActionResult> Dashboard()
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            var contactos = await _context.Contactos.ToListAsync();

            // Si no hay contactos, enviar valores vacíos a la vista
            if (!contactos.Any())
            {
                ViewBag.TotalContactos = 0;
                ViewBag.EdadPromedio   = 0;
                ViewBag.MasJoven       = null;
                ViewBag.MayorEdad      = null;
                ViewBag.RangoEdades    = new Dictionary<string, int>();
                ViewBag.Recientes      = new List<Contacto>();
                return View();
            }

            // ── Estadísticas generales ────────────────────────
            var edades = contactos.Select(c => c.Edad).ToList();

            ViewBag.TotalContactos = contactos.Count;
            ViewBag.EdadPromedio   = Math.Round(edades.Average(), 1);
            ViewBag.MasJoven       = contactos.OrderByDescending(c => c.FechaNacimiento).First();
            ViewBag.MayorEdad      = contactos.OrderBy(c => c.FechaNacimiento).First();

            // ── Distribución por rangos de edad ───────────────
            // Usada para las barras de progreso en el Dashboard
            ViewBag.RangoEdades = new Dictionary<string, int>
            {
                { "0-17",  contactos.Count(c => c.Edad <= 17) },
                { "18-30", contactos.Count(c => c.Edad >= 18 && c.Edad <= 30) },
                { "31-45", contactos.Count(c => c.Edad >= 31 && c.Edad <= 45) },
                { "46-60", contactos.Count(c => c.Edad >= 46 && c.Edad <= 60) },
                { "60+",   contactos.Count(c => c.Edad > 60) }
            };

            // ── Últimos 5 contactos registrados ───────────────
            ViewBag.Recientes = contactos.TakeLast(5).ToList();

            return View();
        }


        // ════════════════════════════════════════════════════════
        // READ - Listar todos los contactos
        // ════════════════════════════════════════════════════════

        public async Task<IActionResult> Index()
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            // Traer todos los contactos de la BD
            return View(await _context.Contactos.ToListAsync());
        }


        // ════════════════════════════════════════════════════════
        // CREATE - Registrar nuevo contacto
        // ════════════════════════════════════════════════════════

        // GET: Muestra el formulario vacío para registrar
        public IActionResult Crear()
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Recibe y guarda el nuevo contacto en la BD
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección contra ataques CSRF
        public async Task<IActionResult> Crear(Contacto contacto)
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            // Validar que no exista otro contacto con la misma cédula
            if (await _context.Contactos.AnyAsync(c => c.Cedula == contacto.Cedula))
                ModelState.AddModelError("Cedula",
                    "Ya existe un contacto con esa cédula.");

            // Validar que la fecha de nacimiento sea razonable
            ValidarFecha(contacto.FechaNacimiento);

            if (ModelState.IsValid)
            {
                // Insertar en la BD y confirmar la transacción
                _context.Contactos.Add(contacto);
                await _context.SaveChangesAsync();

                // Mensaje de éxito que se muestra en la vista siguiente
                TempData["Exito"] = "Contacto registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores de validación, volver al formulario con los datos
            return View(contacto);
        }


        // ════════════════════════════════════════════════════════
        // UPDATE - Editar contacto existente
        // ════════════════════════════════════════════════════════

        // GET: Carga el formulario con los datos actuales del contacto
        public async Task<IActionResult> Editar(int? id)
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            // Buscar el contacto por su ID en la BD
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto == null) return NotFound();

            return View(contacto);
        }

        // POST: Recibe los datos actualizados y los guarda en la BD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Contacto contacto)
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            if (id != contacto.Id) return NotFound();

            // Validar cédula duplicada excluyendo el contacto actual
            if (await _context.Contactos.AnyAsync(c => c.Cedula == contacto.Cedula && c.Id != id))
                ModelState.AddModelError("Cedula",
                    "Ya existe otro contacto con esa cédula.");

            // Validar que la fecha de nacimiento sea razonable
            ValidarFecha(contacto.FechaNacimiento);

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el registro en la BD
                    _context.Update(contacto);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Contacto actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Manejo de concurrencia: si el registro fue eliminado
                    // por otro usuario mientras se editaba
                    if (!_context.Contactos.Any(c => c.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(contacto);
        }


        // ════════════════════════════════════════════════════════
        // DELETE - Eliminar contacto
        // El modal de confirmación está en Index.cshtml
        // Solo existe el POST porque no hay página de confirmación aparte
        // ════════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            var contacto = await _context.Contactos.FindAsync(id);

            if (contacto != null)
            {
                // Eliminar el registro de la BD
                _context.Contactos.Remove(contacto);
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"Contacto {contacto.NombreCompleto} eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }


        // ════════════════════════════════════════════════════════
        // EXPORTAR CSV
        // Genera y descarga un archivo .csv con todos los contactos
        // Compatible con Excel en español (separador punto y coma)
        // ════════════════════════════════════════════════════════

        public async Task<IActionResult> ExportarCsv()
        {
            if (!EstaLogueado())
                return RedirectToAction("Login", "Account");

            var contactos = await _context.Contactos.ToListAsync();
            var sb = new StringBuilder();

            // ── Encabezado del CSV ────────────────────────────
            sb.AppendLine("Cedula;Nombre;Apellidos;Fecha Nacimiento;Edad;Telefono;Direccion");

            // ── Filas de datos ────────────────────────────────
            foreach (var c in contactos)
                sb.AppendLine($"{c.Cedula};{c.Nombre};{c.Apellidos};" +
                              $"{c.FechaNacimiento:yyyy-MM-dd};{c.Edad};" +
                              $"{c.Telefono};\"{c.Direccion}\"");

            // UTF-8 con BOM para que Excel muestre tildes y ñ correctamente
            var bytes = Encoding.UTF8.GetPreamble()
                        .Concat(Encoding.UTF8.GetBytes(sb.ToString()))
                        .ToArray();

            // Nombre del archivo incluye la fecha de exportación
            var fecha = DateTime.Now.ToString("yyyy-MM-dd");
            return File(bytes, "text/csv", $"contactos_manolo_{fecha}.csv");
        }
    }
}