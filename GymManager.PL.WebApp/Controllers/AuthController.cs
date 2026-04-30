using GymManager.BL.BC;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymManager.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioBC _usuarioBC;

        public AuthController(IConfiguration configuration)
        {
            _usuarioBC = new UsuarioBC(
                configuration.GetConnectionString("DefaultConnection")!);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            if (!string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Login");

            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var (usuario, error) = await _usuarioBC.ValidarCredenciales(email, password);

            if (error != null)
            {
                ViewBag.Error = error;
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,  usuario!.nombre),
                new Claim(ClaimTypes.Email, usuario.email),
                new Claim(ClaimTypes.Role,  usuario.rol),
                new Claim("UsuarioId",      usuario.usuario_id.ToString())
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}