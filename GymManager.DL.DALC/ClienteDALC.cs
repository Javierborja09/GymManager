using GymManager.BL.BE;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GymManager.DL.DALC
{
    public class ClienteDALC
    {
        private readonly string _connectionString;

        public ClienteDALC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ClienteBE>> ListarClientes(string? dni)
        {
            var lista = new List<ClienteBE>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarClientes", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Dni", (object?)dni ?? DBNull.Value);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new ClienteBE
                        {
                            cliente_id = (long)reader["cliente_id"],
                            dni = reader["dni"].ToString()!,
                            nombre = reader["nombre"].ToString()!,
                            apellido = reader["apellido"].ToString()!,
                            telefono = reader["telefono"].ToString(),
                            email = reader["email"].ToString(),
                            estado = reader["estado"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<ClienteBE?> ObtenerPorId(long id)
        {
            ClienteBE? cliente = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerClientePorId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteId", id);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cliente = new ClienteBE
                        {
                            cliente_id = (long)reader["cliente_id"],
                            dni = reader["dni"].ToString()!,
                            nombre = reader["nombre"].ToString()!,
                            apellido = reader["apellido"].ToString()!,
                            telefono = reader["telefono"].ToString(),
                            email = reader["email"].ToString(),
                            fecha_registro = (DateTime)reader["fecha_registro"],
                            estado = reader["estado"].ToString()!
                        };
                    }
                }
            }
            return cliente;
        }

        public async Task RegistrarCliente(ClienteBE cliente)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarCliente", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Dni", cliente.dni);
                cmd.Parameters.AddWithValue("@Nombre", cliente.nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.apellido);
                cmd.Parameters.AddWithValue("@Telefono", (object?)cliente.telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)cliente.email ?? DBNull.Value);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task ActualizarCliente(ClienteBE cliente)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarCliente", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteId", cliente.cliente_id);
                cmd.Parameters.AddWithValue("@Dni", cliente.dni);
                cmd.Parameters.AddWithValue("@Nombre", cliente.nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.apellido);
                cmd.Parameters.AddWithValue("@Telefono", (object?)cliente.telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)cliente.email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", cliente.estado);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}