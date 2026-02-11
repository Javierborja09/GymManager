using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // INDISPENSABLE para evitar el error de la imagen
using GymManager.Data;
using System.Security.Claims;

namespace GymManager.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DBConnection _context;

        public DashboardController(DBConnection context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var mesActual = hoy.Month;
            var anioActual = hoy.Year;

            // 1. Estadísticas básicas
            ViewBag.SociosActivos = await _context.Clientes.CountAsync(c => c.estado == "Activo");
            ViewBag.BajoStock = await _context.Productos.CountAsync(p => p.stock_actual <= 5);
            ViewBag.PorVencer = await _context.Matriculas
                .CountAsync(m => m.fecha_fin >= hoy && m.fecha_fin <= hoy.AddDays(7));

            // 2. RECAUDACIÓN DEL DÍA (Desglosada)
            decimal membresiasHoy = await _context.Matriculas
                .Where(m => m.fecha_inicio >= hoy)
                .SumAsync(m => (decimal?)m.monto_pagado) ?? 0;

            decimal productosHoy = await _context.Ventas
                .Where(v => v.fecha_venta >= hoy)
                .SumAsync(v => (decimal?)v.total_venta) ?? 0;

            ViewBag.MembresiasHoy = membresiasHoy;
            ViewBag.ProductosHoy = productosHoy;
            ViewBag.TotalRecaudadoHoy = membresiasHoy + productosHoy; // Suma total del día

            // 3. Lógica de Meta Mensual (Acumulado del mes)
            var metaMes = await _context.MetasMensuales
                .FirstOrDefaultAsync(m => m.mes == mesActual && m.anio == anioActual);

            decimal ingresosMembresiasMes = await _context.Matriculas
                .Where(m => m.fecha_inicio.Month == mesActual && m.fecha_inicio.Year == anioActual)
                .SumAsync(m => m.monto_pagado);

            decimal ingresosProductosMes = await _context.Ventas
                .Where(v => v.fecha_venta.Month == mesActual && v.fecha_venta.Year == anioActual)
                .SumAsync(v => v.total_venta);

            decimal totalMensual = ingresosMembresiasMes + ingresosProductosMes;

            ViewBag.MetaMonto = metaMes?.objetivo_monto ?? 0;
            ViewBag.MetaRecaudado = totalMensual;
            ViewBag.MetaPorcentaje = ViewBag.MetaMonto > 0 ? (int)((totalMensual / ViewBag.MetaMonto) * 100) : 0;

            return View();
        }
    }
}