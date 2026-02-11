using GymManager.Data;
using GymManager.Models;
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
            // Detecta automáticamente el año del sistema (ahora 2026)
            int anioActual = DateTime.Now.Year;

            var metas = await _context.MetasMensuales
                .Where(m => m.anio == anioActual)
                .OrderBy(m => m.mes)
                .ToListAsync();

            foreach (var meta in metas)
            {
                // Suma dinámica de ingresos del mes y año actual
                var totalMatriculas = await _context.Matriculas
                    .Where(m => m.fecha_inicio.Month == meta.mes && m.fecha_inicio.Year == anioActual)
                    .SumAsync(m => m.monto_pagado);

                var totalVentas = await _context.Ventas
                    .Where(v => v.fecha_venta.Month == meta.mes && v.fecha_venta.Year == anioActual)
                    .SumAsync(v => v.total_venta);

                meta.RecaudadoReal = totalMatriculas + totalVentas;
            }

            ViewBag.Anio = anioActual;
            return View(metas);
        }

        // Métodos para editar la meta (Asegúrate de tenerlos)
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