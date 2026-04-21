using GymManager.BL.BE;
using GymManager.DL.DALC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BC
{
    public class MetasMensualesBC
    {
        private readonly MetaMenDALC _metaDalc;
        private readonly MatriculaDALC _matriculaDalc; 
        private readonly VentasDALC _ventaDalc;

        public MetasMensualesBC(string connectionString)
        {
            _metaDalc = new MetaMenDALC(connectionString);
            _matriculaDalc = new MatriculaDALC(connectionString);
            _ventaDalc = new VentasDALC(connectionString);
        }

        public async Task<List<MetasMensualesBE>> ListarMetasConProgreso(int anio)
        {
            var metasDelAnio = (await _metaDalc.Listar())
                             .Where(m => m.anio == anio)
                             .OrderBy(m => m.mes)
                             .ToList();

            foreach (var meta in metasDelAnio)
            {
                // Pedimos los totales directamente a las DALCs
                decimal totalM = await _matriculaDalc.ObtenerRecaudacionMatriculas(meta.mes, anio);
                decimal totalV = await _ventaDalc.ObtenerRecaudacionVentas(meta.mes, anio);

                meta.RecaudadoReal = totalM + totalV;
            }

            return metasDelAnio;
        }

        public async Task<MetasMensualesBE> ObtenerPorId(int id)
        {
            return await _metaDalc.ObtenerPorId(id);
        }
        public async Task<bool> ActualizarMeta(MetasMensualesBE meta)
        {
            // VALIDACIONES DE NEGOCIO
            if (meta.objetivo_monto < 0)
                throw new Exception("El monto objetivo no puede ser una cifra negativa.");

            if (meta.objetivo_monto > 1000000) 
                throw new Exception("El monto objetivo excede el límite operativo permitido.");

            if (!string.IsNullOrEmpty(meta.descripcion) && meta.descripcion.Length > 100)
                throw new Exception("La descripción es demasiado larga (máximo 100 caracteres).");

            return await _metaDalc.Actualizar(meta);
        }
    }
}
