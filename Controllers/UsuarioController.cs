using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly DBConnection _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioController(DBConnection context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        // GET: Lista de Usuarios
        public async Task<IActionResult> Index()
        {
            // Listamos todos los usuarios registrados en el sistema
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // GET: Formulario de Creación
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guardar Usuario con Hash de Contraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario, string password)
        {
            if (ModelState.IsValid)
            {
                // Hashear la contraseña antes de guardar para seguridad
                usuario.password_hash = _passwordHasher.HashPassword(usuario, password);
                usuario.fecha_creacion = DateTime.Now; 
                usuario.activo = true;

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Formulario de Edición
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Actualizar Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Usuario usuario, string newPassword)
        {
            if (id != usuario.usuario_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Si el admin escribe una nueva contraseña, actualizamos el Hash
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        usuario.password_hash = _passwordHasher.HashPassword(usuario, newPassword);
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.usuario_id == usuario.usuario_id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // POST: Alternar Estado (Activo/Inactivo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.activo = !usuario.activo;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}