builder.Services.AddSession(options =>
{
options.IdleTimeout = TimeSpan.FromMinutes(30);
options.Cookie.HttpOnly = true;
options.Cookie.IsEssential = true;
});
var app = builder.Build();
// Crear y migrar la BD automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();
}
if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();       // <-- IMPORTANTE: antes de Authorization
app.UseAuthorization();
// La ruta por defecto va al Login
app.MapControllerRoute(
name: "default",
pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();