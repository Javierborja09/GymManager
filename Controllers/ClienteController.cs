using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Data;
using GymManager.Models;
using GymManager.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace GymManager.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly DBConnection _context;

        public ClienteController(DBConnection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string buscarDni)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(buscarDni))
            {
                query = query.Where(c => c.dni.Contains(buscarDni));
            }

            var clientes = await query
                .Select(c => new ClienteDTO
                {
                    cliente_id = c.cliente_id,
                    dni = c.dni,
                    nombre = c.nombre,
                    apellido = c.apellido,
                    estado = c.estado
                })
                .ToListAsync();

            ViewData["FiltroDni"] = buscarDni;
            return View(clientes);
        }

        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Cliente cliente)
        {
            if (id != cliente.cliente_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clientes.Any(e => e.cliente_id == cliente.cliente_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.RegistrarClienteSP(
                        cliente.dni,
                        cliente.nombre,
                        cliente.apellido,
                        cliente.telefono ?? "",
                        cliente.email ?? ""
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar: " + ex.Message);
                }
            }
            return View(cliente);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.cliente_id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }
    }
}