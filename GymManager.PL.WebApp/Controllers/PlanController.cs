using GymManager.Data;
using GymManager.Models;
using GymManager.Web.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlanController : Controller
    {
        private readonly DBConnection _context;

        public PlanController(DBConnection context)
        {
            _context = context;
        }

        // Listado de planes actuales
        public async Task<IActionResult> Index()
        {
            var planesDto = await _context.Planes
                .Select(p => new PlanDTO
                {
                    PlanId = p.plan_id,
                    NombrePlan = p.nombre_plan,
                    DuracionDias = p.duracion_dias,
                    Precio = p.precio
                })
                .ToListAsync();

            return View(planesDto);
        }

        // Formulario para crear un nuevo plan
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Create(Plan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Nuevo plan de entrenamiento creado.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: Plan/Edit/5
        public async Task<IActionResult>
            Edit(long? id)
        {
            if (id == null) return NotFound();
            var plan = await _context.Planes.FindAsync(id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Edit(long id, Plan plan)
        {
            if (id != plan.plan_id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}
