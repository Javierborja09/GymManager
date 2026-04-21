using GymManager.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.DL.DALC
{
    public class ClienteDALC
    {
        private readonly string _connectionString;
        public ClienteDALC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ClienteBE>> ListarClientes(string dni)
        {
            var lista = new List<ClienteBE>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarClientes", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Dni", (object)dni ?? DBNull.Value);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new ClienteBE
                        {
                            cliente_id = (long)reader["cliente_id"],
                            dni = reader["dni"].ToString(),
                            nombre = reader["nombre"].ToString(),
                            apellido = reader["apellido"].ToString(),
                            telefono = reader["telefono"].ToString(),
                            email = reader["email"].ToString(),
                            estado = reader["estado"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

    }
}
