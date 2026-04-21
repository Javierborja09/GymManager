using GymManager.BL.BE;
using GymManager.DL.DALC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BC
{
    public class MatriculaBC
    {
        private readonly MatriculaDALC _matriculaDALC;
        private readonly PlanDALC _planDALC; 
        public MatriculaBC(string connectionString)
        {
            _matriculaDALC = new MatriculaDALC(connectionString);
            _planDALC = new PlanDALC(connectionString);
        }

        public async Task<List<MatriculaBE>> ListarMatriculas()
        {
            return await _matriculaDALC.ListarMatriculas();
        }
        public async Task<MatriculaBE> ObtenerDetalleCompleto(long id)
        {
            return await _matriculaDALC.ObtenerPorId(id);
        }

        public async Task RegistrarMatricula(MatriculaBE matricula)
        {
            // 1. Obtenemos el plan desde la DALC para calcular fechas
            var plan = await _planDALC.ObtenerPorID(matricula.plan_id);
            if (plan == null) throw new Exception("El plan seleccionado no existe.");

            // 2. Lógica de negocio (Cálculos automáticos)
            matricula.fecha_inicio = DateTime.Now;
            matricula.fecha_fin = matricula.fecha_inicio.AddDays(plan.duracion_dias);
            matricula.monto_pagado = plan.precio;

            // 3. Mandamos a guardar
            await _matriculaDALC.InsertarMatricula(matricula);
        }
        public async Task<decimal> ObtenerRecaudacionHoy()
        {
           
            var todas = await _matriculaDALC.ListarMatriculas();
            var recaudado = todas
                .Where(m => m.fecha_inicio.Date == DateTime.Today)
                .Sum(m => m.monto_pagado);
            return recaudado;
        }

    }
}
