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
    public class MetaMenDALC
    {
        private readonly string _connectionString;

        public MetaMenDALC(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<MetasMensualesBE>> Listar()
        {
            var lista = new List<MetasMensualesBE>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ListarMetasMensuales", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new MetasMensualesBE
                            {
                                meta_id = reader.GetInt32(reader.GetOrdinal("meta_id")),
                                mes = reader.GetInt32(reader.GetOrdinal("mes")),
                                anio = reader.GetInt32(reader.GetOrdinal("anio")),
                                objetivo_monto = reader.GetDecimal(reader.GetOrdinal("objetivo_monto")),
                                descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion"))
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<MetasMensualesBE> ObtenerPorId(int id)
        {
            MetasMensualesBE meta = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerMetaMensualPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@meta_id", id);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            meta = new MetasMensualesBE
                            {
                                meta_id = reader.GetInt32(reader.GetOrdinal("meta_id")),
                                mes = reader.GetInt32(reader.GetOrdinal("mes")),
                                anio = reader.GetInt32(reader.GetOrdinal("anio")),
                                objetivo_monto = reader.GetDecimal(reader.GetOrdinal("objetivo_monto")),
                                descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString(reader.GetOrdinal("descripcion"))
                            };
                        }
                    }
                }
            }
            return meta;
        }

        public async Task<bool> Actualizar(MetasMensualesBE meta)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ActualizarMetaMensual", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@meta_id", meta.meta_id);
                    cmd.Parameters.AddWithValue("@objetivo_monto", meta.objetivo_monto);
                    cmd.Parameters.AddWithValue("@descripcion", meta.descripcion ?? (object)DBNull.Value);

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}
