using GymManager.BL.BE;
using GymManager.DL.DALC;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.BL.BC
{
    
    public class PlanBC
    {
        private readonly PlanDALC _dalc;

        public PlanBC(string connectionString)
        {
            _dalc = new PlanDALC(connectionString);
        }

        public async Task<List<PlanBE>> Listar() => await _dalc.Listar();

        public async Task<PlanBE> ObtenerPorID(long id) => await _dalc.ObtenerPorID(id);

        public async Task<bool> Guardar(PlanBE plan)
        {
            if (plan.precio < 0) throw new Exception("El precio no puede ser negativo.");
            if (plan.duracion_dias <= 0) throw new Exception("La duración debe ser al menos de 1 día.");

            if (plan.plan_id == 0)
                return await _dalc.Insertar(plan);
            else
                return await _dalc.Actualizar(plan);
        }
    }
}
