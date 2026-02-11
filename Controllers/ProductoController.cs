using GymManager.Data;
using GymManager.Models;
using GymManager.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class ProductoController : Controller
    {
        private readonly DBConnection _context;

        public ProductoController(DBConnection context)
        {
            _context = context;
        }

        // GET: Producto
        // Lista todos los productos y permite búsqueda opcional
        public async Task<IActionResult> Index(string buscar)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.nombre.Contains(buscar) || p.categoria.Contains(buscar));
            }

            // Proyección directa al DTO para mejorar el rendimiento
            var productosDto = await query
                .Select(p => new ProductoDTO
                {
                    ProductoId = p.producto_id,
                    Nombre = p.nombre,
                    PrecioVenta = p.precio_venta,
                    StockActual = p.stock_actual,
                    Categoria = p.categoria
                })
                .ToListAsync();

            return View(productosDto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar el producto: " + ex.Message);
                }
            }
            return View(producto);
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Producto producto)
        {
            if (id != producto.producto_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Productos.Any(e => e.producto_id == producto.producto_id)) return NotFound();
                    else throw;
                }
            }
            return View(producto);
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.producto_id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Producto/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}