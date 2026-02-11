using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Data;
using GymManager.Models.DTOs; 
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace GymManager.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly DBConnection _context;

        public VentaController(DBConnection context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ventas = await _context.Ventas
                .OrderByDescending(v => v.venta_id)
                .Select(v => new VentaDTO
                {
                    VentaId = v.venta_id,
                    Fecha = v.fecha_venta,
                    ClienteNombre = v.Cliente != null ? v.Cliente.nombre + " " + v.Cliente.apellido : "Público General",
                    UsuarioNombre = v.Usuario != null ? v.Usuario.nombre : "Sistema",
                    Total = v.total_venta,
                    Productos = v.DetalleVentas.Select(d => new VentaDetalleDTO
                    {
                        ProductoNombre = d.Producto!.nombre,
                        Cantidad = d.cantidad
                    }).ToList()
                })
                .ToListAsync();

            return View(ventas);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Productos = await _context.Productos.Where(p => p.stock_actual > 0).ToListAsync();
            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long? cliente_id, List<VentaItemDTO> items)
        {
            if (items == null || !items.Any()) return RedirectToAction(nameof(Create));

            try
            {
                var usuarioId = long.Parse(User.FindFirst("UsuarioId")!.Value);
                decimal granTotal = 0;

                // Preparamos la lista para el JSON
                var listaParaJson = new List<object>();

                foreach (var item in items)
                {
                    var p = await _context.Productos.FindAsync(item.producto_id);
                    if (p != null)
                    {
                        granTotal += (p.precio_venta * item.cantidad);
                        listaParaJson.Add(new
                        {
                            producto_id = item.producto_id,
                            cantidad = item.cantidad,
                            precio_unitario = p.precio_venta
                        });
                    }
                }

                // Serializamos a JSON
                string json = JsonSerializer.Serialize(listaParaJson);

                // Una sola llamada al SP
                await _context.RegistrarVentaJsonSP(cliente_id, usuarioId, granTotal, json);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }
    }
}