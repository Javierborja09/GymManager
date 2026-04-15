using GymManager.Data;
using GymManager.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
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

            var dto = new DashboardDTO
            {
                SociosActivos = await _context.Clientes.CountAsync(c => c.estado == "Activo"),
                BajoStock = await _context.Productos.CountAsync(p => p.stock_actual <= 5),
                PorVencer = await _context.Matriculas.CountAsync(m => m.fecha_fin >= hoy && m.fecha_fin <= hoy.AddDays(7)),

                MesNombre = hoy.ToString("MMMM").ToUpper(),

                MembresiasHoy = await _context.Matriculas
                    .Where(m => m.fecha_inicio >= hoy)
                    .SumAsync(m => (decimal?)m.monto_pagado) ?? 0,

                ProductosHoy = await _context.Ventas
                    .Where(v => v.fecha_venta >= hoy)
                    .SumAsync(v => (decimal?)v.total_venta) ?? 0
            };

            var metaMes = await _context.MetasMensuales
                .FirstOrDefaultAsync(m => m.mes == mesActual && m.anio == anioActual);

            decimal membresiasMes = await _context.Matriculas
                .Where(m => m.fecha_inicio.Month == mesActual && m.fecha_inicio.Year == anioActual)
                .SumAsync(m => m.monto_pagado);

            decimal productosMes = await _context.Ventas
                .Where(v => v.fecha_venta.Month == mesActual && v.fecha_venta.Year == anioActual)
                .SumAsync(v => v.total_venta);

            dto.MetaMonto = metaMes?.objetivo_monto ?? 0;
            dto.MetaRecaudado = membresiasMes + productosMes;

            return View(dto); 
        }
    }
}