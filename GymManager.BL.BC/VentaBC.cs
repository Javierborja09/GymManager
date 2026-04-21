using GymManager.DL.DALC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BC
{
    public class VentaBC
    {
        private readonly VentasDALC _ventaDALC;

        public VentaBC(string connectionString)
        {
            _ventaDALC = new VentasDALC(connectionString);
        }

        public async Task<decimal> CalcularVentasHoy()
        {
            return await _ventaDALC.ObtenerTotalPorFecha(DateTime.Today);
        }

    }
}
