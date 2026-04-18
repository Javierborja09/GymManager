using GymManager.BL.BC;
using GymManager.BL.BE;
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
        private readonly PlanBC _planBC;
        public PlanController(PlanBC planBC)
        {
           _planBC = planBC;
        }

        // Listado de planes actuales
        public async Task<IActionResult> Index()
        {
            var planes = await _planBC.Listar();

            // Mapeo a DTO para la vista
            var planesDto = planes.Select(p => new PlanDTO
            {
                PlanId = p.plan_id,
                NombrePlan = p.nombre_plan,
                DuracionDias = p.duracion_dias,
                Precio = p.precio
            }).ToList();

            return View(planesDto);
        }

        // Formulario para crear un nuevo plan
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanBE planBE)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _planBC.Guardar(planBE);
                    TempData["Success"] = "Nuevo plan de entrenamiento creado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(planBE);
        }

        // GET: Plan/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            var plan = await _planBC.ObtenerPorID(id.Value);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, PlanBE planBE)
        {
            if (id != planBE.plan_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _planBC.Guardar(planBE);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(planBE);
        }
    }
}
