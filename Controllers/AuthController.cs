using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymManager.Data;
using GymManager.Models;
using System.Security.Claims;

namespace GymManager.Controllers
{
    public class AuthController : Controller
    {
        private readonly DBConnection _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public AuthController(DBConnection context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Dashboard");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.email == email && u.activo == true);

            if (usuario != null)
            {
                var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.password_hash, password);

                if (resultado == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.nombre),
                        new Claim(ClaimTypes.Email, usuario.email),
                        new Claim(ClaimTypes.Role, usuario.rol),
                        new Claim("UsuarioId", usuario.usuario_id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    // 3. Siempre redirigir al Dashboard para asegurar limpieza
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.Error = "Credenciales incorrectas o cuenta inactiva.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}