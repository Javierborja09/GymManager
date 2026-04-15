using GymManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GymManager.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. CONFIGURACIÓN DE LA BASE DE DATOS
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DBConnection>(options =>
                options.UseSqlServer(connectionString));

            // 2. CONFIGURACIÓN DE AUTENTICACIÓN POR COOKIES
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login"; // Destino si no hay sesión
                    options.AccessDeniedPath = "/Auth/AccessDenied"; // Destino si falta permiso
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duración de 1 hora
                    options.SlidingExpiration = true; // Renueva el tiempo si el usuario está activo
                });

            // 3. AGREGAR SERVICIOS MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();;

            // 4. CAPTURA DE ERRORES 404 
            app.UseStatusCodePagesWithReExecute("/Dashboard/Error{0}");

            // 5. CONFIGURACIÓN DEL PIPELINE (Middlewares)
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Dashboard/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 6. ACTIVAR SEGURIDAD (Orden obligatorio)
            app.UseAuthentication();
            app.UseAuthorization(); 


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}