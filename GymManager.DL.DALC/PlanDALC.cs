using GymManager.BL.BE;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GymManager.DL.DALC
{
    public class PlanDALC
    {
        private readonly string _connectionString;

        public PlanDALC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<PlanBE>> Listar()
        {
            var lista = new List<PlanBE>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarPlanes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new PlanBE
                            {
                                plan_id = reader.GetInt64(reader.GetOrdinal("plan_id")),
                                nombre_plan = reader.GetString(reader.GetOrdinal("nombre_plan")),
                                duracion_dias = reader.GetInt32(reader.GetOrdinal("duracion_dias")),
                                precio = reader.GetDecimal(reader.GetOrdinal("precio"))
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<PlanBE> ObtenerPorID(long id)
        {
            PlanBE plan = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPlanPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@plan_id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            plan = new PlanBE
                            {
                                plan_id = reader.GetInt64(reader.GetOrdinal("plan_id")),
                                nombre_plan = reader.GetString(reader.GetOrdinal("nombre_plan")),
                                duracion_dias = reader.GetInt32(reader.GetOrdinal("duracion_dias")),
                                precio = reader.GetDecimal(reader.GetOrdinal("precio"))
                            };
                        }
                    }
                }
            }
            return plan;
        }

        public async Task<bool> Insertar(PlanBE plan)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertarPlan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre_plan", plan.nombre_plan);
                    cmd.Parameters.AddWithValue("@duracion_dias", plan.duracion_dias);
                    cmd.Parameters.AddWithValue("@precio", plan.precio);

                    await conn.OpenAsync();
                    // Como el SP hace un SELECT SCOPE_IDENTITY, podrías usar ExecuteScalar si quisieras el nuevo ID
                    int filas = await cmd.ExecuteNonQueryAsync();
                    return filas > 0;
                }
            }
        }

        public async Task<bool> Actualizar(PlanBE plan)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarPlan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@plan_id", plan.plan_id);
                    cmd.Parameters.AddWithValue("@nombre_plan", plan.nombre_plan);
                    cmd.Parameters.AddWithValue("@duracion_dias", plan.duracion_dias);
                    cmd.Parameters.AddWithValue("@precio", plan.precio);

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}
