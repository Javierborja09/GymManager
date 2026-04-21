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
    public class MatriculaDALC
    {
        private readonly string _connectionString;

        public MatriculaDALC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<MatriculaBE>> ListarMatriculas()
        {
            var lista = new List<MatriculaBE>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarMatriculas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new MatriculaBE
                            {
                                matricula_id = reader.GetInt64(reader.GetOrdinal("matricula_id")),
                                Cliente = new ClienteBE
                                {
                                    nombre = reader["nombre"].ToString(),
                                    apellido = reader["apellido"].ToString(),
                                    dni = reader["dni"].ToString()
                                },
                                Plan = new PlanBE
                                {
                                    nombre_plan = reader["nombre_plan"].ToString()
                                },
                                fecha_inicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                                fecha_fin = reader.GetDateTime(reader.GetOrdinal("fecha_fin")),
                                monto_pagado = reader.GetDecimal(reader.GetOrdinal("monto_pagado"))
                            });
                        }
                    }
                }
            }
            return lista;
        }
        // Obtener por ID
        public async Task<MatriculaBE> ObtenerPorId(long id)
        {
            MatriculaBE matricula = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerMatriculaPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@matricula_id", id);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            matricula = new MatriculaBE
                            {
                                matricula_id = (long)reader["matricula_id"],
                                fecha_inicio = (DateTime)reader["fecha_inicio"],
                                fecha_fin = (DateTime)reader["fecha_fin"],
                                monto_pagado = (decimal)reader["monto_pagado"],
                                // Mapeo del objeto Cliente relacionado
                                Cliente = new ClienteBE
                                {
                                    nombre = reader["nombre"].ToString(),
                                    apellido = reader["apellido"].ToString(),
                                    dni = reader["dni"].ToString(),
                                    telefono = reader["telefono"].ToString()
                                },
                                // Mapeo del objeto Plan relacionado
                                Plan = new PlanBE
                                {
                                    nombre_plan = reader["nombre_plan"].ToString()
                                }
                            };
                        }
                    }
                }
            }
            return matricula;
        }

        // Insertar Matricula
        public async Task InsertarMatricula(MatriculaBE matricula)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertarMatricula", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cliente_id", matricula.cliente_id);
                    cmd.Parameters.AddWithValue("@plan_id", matricula.plan_id);
                    cmd.Parameters.AddWithValue("@fecha_inicio", matricula.fecha_inicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", matricula.fecha_fin);
                    cmd.Parameters.AddWithValue("@monto_pagado", matricula.monto_pagado);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
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
