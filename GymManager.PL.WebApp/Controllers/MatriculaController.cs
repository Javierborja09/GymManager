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
    [Authorize] // Requiere login para matricular
    public class MatriculaController : Controller
    {
        private readonly PlanBC _planBC;
        private readonly MatriculaBC _matriculaBC;
        private readonly ClienteBC _clienteBC;
        public MatriculaController(MatriculaBC matriculaBC, PlanBC planBC, ClienteBC clienteBC)
        {
            _matriculaBC = matriculaBC;
            _planBC = planBC;
            _clienteBC = clienteBC;
        }

        // Listado de membresías actuales
        public async Task<IActionResult> Index()
        {
            var listaMatriculas = await _matriculaBC.ListarMatriculas();
            var matriculasDto = listaMatriculas
                .Select(m => new MatriculaDTO
                {
                    matricula_id = m.matricula_id,
                    SocioNombre = m.Cliente!.nombre + " " + m.Cliente.apellido,
                    SocioDni = m.Cliente.dni,
                    PlanNombre = m.Plan!.nombre_plan,
                    FechaInicio = m.fecha_inicio,
                    FechaFin = m.fecha_fin,
                    MontoPagado = m.monto_pagado
                })
                .ToList();

            return View(matriculasDto);
        }

        // Formulario de Nueva Matrícula
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await _clienteBC.ListarActivos();
            ViewBag.Planes = await _planBC.Listar();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatriculaBE matricula)
        {
            try
            {

                await _matriculaBC.RegistrarMatricula(matricula);

                TempData["Success"] = "¡Membresía activada correctamente!";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al procesar la matrícula: " + ex.Message;
            }

            ViewBag.Clientes = await _clienteBC.ListarActivos();
            ViewBag.Planes = await _planBC.Listar();
            return View(matricula);
        }

        // GET: Matricula/Details/5
        public async Task<IActionResult> Details(long id)
        {
            // Obtenemos la entidad completa desde la BC
            var matriculaBE = await _matriculaBC.ObtenerDetalleCompleto(id);

            if (matriculaBE == null) return NotFound();

            return View(matriculaBE);
        }
    }
}