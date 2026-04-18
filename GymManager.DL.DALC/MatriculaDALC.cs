using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.DL.DALC
{
    public class MatriculaDALC
    {
        private readonly string _connectionString;

        public MatriculaDALC(string connectionString)
        {
            _connectionString = connectionString;
        }


        // Metodo para las Metas mensuales
        public async Task<decimal> ObtenerRecaudacionMatriculas(int mes, int anio)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerTotalMatriculasMensual", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@mes", mes);
                    cmd.Parameters.AddWithValue("@anio", anio);
                    await conn.OpenAsync();
                    return Convert.ToDecimal(await cmd.ExecuteScalarAsync());
                }
            }
        }

    }
}
