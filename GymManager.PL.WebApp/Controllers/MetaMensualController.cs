using GymManager.Data;
using GymManager.Models;
using GymManager.Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace GymManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MetaMensualController : Controller
    {
        private readonly DBConnection _context;

        public MetaMensualController(DBConnection context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            int anioActual = DateTime.Now.Year;

            var metasBase = await _context.MetasMensuales
                .Where(m => m.anio == anioActual)
                .OrderBy(m => m.mes)
                .ToListAsync();

            var metasDto = new List<MetaMensualDTO>();

            foreach (var meta in metasBase)
            {
                // Sumamos ingresos de Matrículas y Ventas de productos
                var totalMatriculas = await _context.Matriculas
                    .Where(m => m.fecha_inicio.Month == meta.mes && m.fecha_inicio.Year == anioActual)
                    .SumAsync(m => (decimal?)m.monto_pagado) ?? 0;

                var totalVentas = await _context.Ventas
                    .Where(v => v.fecha_venta.Month == meta.mes && v.fecha_venta.Year == anioActual)
                    .SumAsync(v => (decimal?)v.total_venta) ?? 0;

                metasDto.Add(new MetaMensualDTO
                {
                    MetaId = meta.meta_id,
                    Mes = meta.mes,
                    Anio = meta.anio,
                    MesNombre = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(meta.mes).ToUpper(),
                    ObjetivoMonto = meta.objetivo_monto,
                    RecaudadoReal = totalMatriculas + totalVentas,
                    Descripcion = meta.descripcion
                });
            }

            ViewBag.Anio = anioActual;
            return View(metasDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var meta = await _context.MetasMensuales.FindAsync(id);
            return meta == null ? NotFound() : View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MetaMensual meta)
        {
            if (id != meta.meta_id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(meta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(meta);
        }
    }
}