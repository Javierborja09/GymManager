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
            // 1. Buscamos al usuario solo por email para verificar su existencia primero
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.email == email);

            if (usuario == null)
            {
                ViewBag.Error = "El correo electrónico no está registrado.";
                return View();
            }

            // 2. Verificamos si la cuenta está desactivada
            if (!usuario.activo)
            {
                ViewBag.Error = "Tu cuenta ha sido desactivada. Contacta al administrador.";
                return View();
            }

            // 3. Verificamos la contraseña
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

                return RedirectToAction("Index", "Dashboard");
            }

            // 4. Si llegó aquí, la contraseña es incorrecta
            ViewBag.Error = "Contraseña incorrecta.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}