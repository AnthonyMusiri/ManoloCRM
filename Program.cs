// ============================================================
// PUNTO DE ENTRADA DE LA APLICACIÓN
// Configura todos los servicios y el pipeline HTTP de ASP.NET
// Este archivo se ejecuta primero cuando arranca la app
// ============================================================

using ManoloCRM.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICIOS ─────────────────────────────────────────────────
// Los servicios se registran aquí y ASP.NET los inyecta
// automáticamente donde se necesiten (inyección de dependencias)

// Habilitar el patrón MVC (Modelos, Vistas y Controladores)
builder.Services.AddControllersWithViews();

// Configurar la base de datos SQLite con Entity Framework Core
// La cadena de conexión viene de appsettings.json
// En producción se cambiaría a SQL Server solo tocando ese archivo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar el sistema de sesiones para el login administrativo
// IdleTimeout: la sesión expira si no hay actividad en 30 minutos
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;  // No accesible desde JavaScript
    options.Cookie.IsEssential = true;  // No requiere consentimiento de cookies
});

// ── CONSTRUCCIÓN DE LA APP ────────────────────────────────────
var app = builder.Build();

// ── MIGRACIONES AUTOMÁTICAS ───────────────────────────────────
// Al iniciar la app, verifica si la BD existe y está actualizada
// Si no existe, la crea. Si tiene migraciones pendientes, las aplica
// Esto evita tener que ejecutar "dotnet ef database update" manualmente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── PIPELINE HTTP ─────────────────────────────────────────────
// Define el orden en que se procesan las peticiones HTTP
// El orden aquí importa - cada middleware pasa la petición al siguiente

if (!app.Environment.IsDevelopment())
{
    // En producción: página de error amigable y cabeceras de seguridad
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirigir HTTP → HTTPS
app.UseStaticFiles();      // Servir archivos de wwwroot (css, js, imágenes)
app.UseRouting();          // Habilitar el sistema de rutas
app.UseSession();          // IMPORTANTE: debe ir antes de UseAuthorization
app.UseAuthorization();    // Habilitar autorización (para futuros [Authorize])

// ── RUTAS ─────────────────────────────────────────────────────
// Ruta por defecto: si el usuario entra a "/", va al Login
// Patrón: /Controlador/Accion/Id(opcional)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// ── INICIAR EL SERVIDOR ───────────────────────────────────────
app.Run();