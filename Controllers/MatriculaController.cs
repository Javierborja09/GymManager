using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymManager.Controllers
{
    [Authorize] // Requiere login para matricular
    public class MatriculaController : Controller
    {
        private readonly DBConnection _context;

        public MatriculaController(DBConnection context)
        {
            _context = context;
        }

        // Listado de membresías actuales
        public async Task<IActionResult> Index()
        {
            var matriculas = await _context.Matriculas
                .Include(m => m.Cliente)
                .Include(m => m.Plan)
                .OrderByDescending(m => m.fecha_inicio)
                .ToListAsync();
            return View(matriculas);
        }

        // Formulario de Nueva Matrícula
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await _context.Clientes.Where(c => c.estado == "Activo").ToListAsync();
            ViewBag.Planes = await _context.Planes.ToListAsync();
            return View();
        }
        // GET: Matricula/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var matricula = await _context.Matriculas
                .Include(m => m.Cliente)
                .Include(m => m.Plan)
                .FirstOrDefaultAsync(m => m.matricula_id == id);

            if (matricula == null) return NotFound();

            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Matricula matricula)
        {
            try
            {
                // 1. Obtener datos del plan para calcular fechas y monto
                var plan = await _context.Planes.FindAsync(matricula.plan_id);
                if (plan == null) return NotFound();

                // 2. Lógica de Negocio: Cálculo automático de fechas
                matricula.fecha_inicio = DateTime.Now;
                matricula.fecha_fin = matricula.fecha_inicio.AddDays(plan.duracion_dias);
                matricula.monto_pagado = plan.precio;

                if (ModelState.IsValid)
                {
                    _context.Add(matricula);
                    await _context.SaveChangesAsync();

                    // Opcional: Actualizar estado del cliente a Activo si era Inactivo
                    var cliente = await _context.Clientes.FindAsync(matricula.cliente_id);
                    if (cliente != null) cliente.estado = "Activo";
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "¡Membresía activada correctamente!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al procesar la matrícula: " + ex.Message;
            }

            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            ViewBag.Planes = await _context.Planes.ToListAsync();
            return View(matricula);
        }
    }
    


}