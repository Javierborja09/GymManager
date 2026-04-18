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
    public class MetaMensualController : Controller
    {
        private readonly MetasMensualesBC _metaBC;

        public MetaMensualController(MetasMensualesBC metaBC)
        {
            _metaBC = metaBC;
        }

        public async Task<IActionResult> Index()
        {
            int anioActual = DateTime.Now.Year;
            var metas = await _metaBC.ListarMetasConProgreso(anioActual);

            // Mapeo al DTO
            var metasDto = metas.Select(m => new MetaMensualDTO
            {
                MetaId = m.meta_id,
                Mes = m.mes,
                Anio = m.anio,
                MesNombre = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(m.mes).ToUpper(),
                ObjetivoMonto = m.objetivo_monto,
                RecaudadoReal = m.RecaudadoReal,
                Descripcion = m.descripcion
            }).ToList();

            ViewBag.Anio = anioActual;
            return View(metasDto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var meta = await _metaBC.ObtenerPorId(id.Value);
            return meta == null ? NotFound() : View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MetasMensualesBE meta)
        {
            if (id != meta.meta_id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _metaBC.ActualizarMeta(meta);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(meta);
        }
    }
}