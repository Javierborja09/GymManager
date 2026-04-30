using GymManager.BL.BE;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GymManager.DL.DALC
{
    public class UsuarioDALC
    {
        private readonly string _connectionString;

        public UsuarioDALC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UsuarioBE?> ObtenerPorEmail(string email)
        {
            UsuarioBE? usuario = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerUsuarioPorEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        usuario = new UsuarioBE
                        {
                            usuario_id = (long)reader["usuario_id"],
                            nombre = reader["nombre"].ToString()!,
                            email = reader["email"].ToString()!,
                            password_hash = reader["password_hash"].ToString()!,
                            rol = reader["rol"].ToString()!,
                            fecha_creacion = (DateTime)reader["fecha_creacion"],
                            activo = (bool)reader["activo"]
                        };
                    }
                }
            }

            return usuario;
        }


    }
}