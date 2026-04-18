using GymManager.BL.BC;
using GymManager.BL.BE;
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
        private readonly ProductosBC _productosBC;

        public ProductoController(ProductosBC productosBC)
        {
            _productosBC = productosBC;
        }

        // GET: Producto
        // Lista todos los productos y permite búsqueda opcional
        public async Task<IActionResult> Index(string buscar)
        {
            var query = await _productosBC.Listar(buscar);

            var productosDto = query.Select(p => new ProductoDTO
                {
                    ProductoId = p.producto_id,
                    Nombre = p.nombre,
                    PrecioVenta = p.precio_venta,
                    StockActual = p.stock_actual,
                    Categoria = p.categoria
                })
                .ToList();
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
        public async Task<IActionResult> Create(ProductoBE productoBE)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productosBC.GuardarProducto(productoBE);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo guardar el producto: " + ex.Message);
                }
            }
            return View(productoBE);
        }

        // GET: Producto/Edit/{id}
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var producto = await _productosBC.ObtenerPorID(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Producto/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ProductoBE productoBE)
        {
            if (id != productoBE.producto_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _productosBC.GuardarProducto(productoBE);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "No se pudo actualizar el producto: " + ex.Message);
                }
            }
            return View(productoBE);
        }

        // GET: Producto/Details/
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var producto = await _productosBC.ObtenerPorID(id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Producto/Delete/

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var producto = await _productosBC.ObtenerPorID(id);
            if (producto != null)
            {
                await _productosBC.Eliminar(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}