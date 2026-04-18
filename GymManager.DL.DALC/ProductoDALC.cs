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
    public class ProductoDALC
    {
        private readonly string _connectionString;

        public ProductoDALC(string connectionString)
        {
            _connectionString = connectionString;
        }
        //Listar
        public async Task<List<ProductoBE>> Listar(string filtro)
        {
            var lista = new List<ProductoBE>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Usamos el SP que sirve para listar y buscar a la vez
                SqlCommand cmd = new SqlCommand("sp_ListarProductos", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Si el filtro es nulo, enviamos DBNull para que el SP ignore el filtro
                cmd.Parameters.AddWithValue("@buscar", (object)filtro ?? DBNull.Value);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new ProductoBE
                        {
                            producto_id = (long)reader["producto_id"],
                            nombre = reader["nombre"].ToString(),
                            precio_venta = (decimal)reader["precio_venta"],
                            stock_actual = (int)reader["stock_actual"],
                            categoria = reader["categoria"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
        //Buscar por id
        public async Task<ProductoBE> ObtenerPorID(long? id)
        {
            ProductoBE producto = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerProductoPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@producto_id", id);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            producto = new ProductoBE();
                            // Usamos GetOrdinal para buscar por nombre de columna, es más seguro
                            producto.producto_id = reader.GetInt64(reader.GetOrdinal("producto_id"));
                            producto.nombre = reader.GetString(reader.GetOrdinal("nombre"));
                            producto.precio_venta = reader.GetDecimal(reader.GetOrdinal("precio_venta"));
                            producto.stock_actual = reader.GetInt32(reader.GetOrdinal("stock_actual"));
                            producto.categoria = reader.GetString(reader.GetOrdinal("categoria"));
                        }
                    }
                }
            }
            return producto;
        }

        // Registrar/Actualizar producto
        public async Task<bool> Guardar(ProductoBE producto)
        {
            bool exito = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GuardarProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Manejo del ID: Si es 0 o null, enviamos DBNull para que el SP lo tome como INSERT
                    cmd.Parameters.AddWithValue("@producto_id",
                        (producto.producto_id == 0 || producto.producto_id == null)
                        ? DBNull.Value
                        : producto.producto_id);

                    cmd.Parameters.AddWithValue("@nombre", producto.nombre);
                    cmd.Parameters.AddWithValue("@precio_venta", producto.precio_venta);
                    cmd.Parameters.AddWithValue("@stock_actual", producto.stock_actual);
                    cmd.Parameters.AddWithValue("@categoria", producto.categoria);

                    await conn.OpenAsync();

                    // ExecuteNonQuery devuelve el número de filas afectadas
                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    exito = filasAfectadas > 0;
                }
            }

            return exito;
        }
        // Eliminar
        public async Task<bool> Eliminar(long id)
        {
            bool eliminado = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_EliminarProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@producto_id", id);

                    await conn.OpenAsync();
                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    eliminado = filasAfectadas > 0;
                }
            }

            return eliminado;
        }




    }
}
