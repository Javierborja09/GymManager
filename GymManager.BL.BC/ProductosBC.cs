using GymManager.BL.BE;
using GymManager.DL.DALC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BC
{
    public class ProductosBC
    {
        private readonly ProductoDALC _dalc;

        public ProductosBC(string connectionString)
        {
            _dalc = new ProductoDALC(connectionString);
        }

        public async Task<List<ProductoBE>> Listar(string buscar)
        {
            if (string.IsNullOrWhiteSpace(buscar) || buscar.Length < 3)
            {
                return await _dalc.Listar(null); // Trae todo
            }

            return await _dalc.Listar(buscar.Trim().ToUpper());
        }
        public async Task<ProductoBE> ObtenerPorID(long? id)
        {
            return await _dalc.ObtenerPorID(id);
        }

        public async Task<bool> GuardarProducto(ProductoBE productoBE)
        {
            // Aquí puedes agregar lógica de negocio antes de mandar a la base de datos
            if (string.IsNullOrEmpty(productoBE.nombre)) return false;

            return await _dalc.Guardar(productoBE);
        }

        // Método que reutiliza la lista general para filtrar
        public async Task<List<ProductoBE>> ObtenerProductosBajoStock(int limite = 5)
        {

            var todosLosProductos = await _dalc.Listar(null);

            if (limite == 0)
            {

                return todosLosProductos.Where(p => p.stock_actual == 0).ToList();
            }
            var bajoStock = todosLosProductos
                .Where(p => p.stock_actual > 0 && p.stock_actual <= limite)
                .OrderBy(p => p.stock_actual)
                .ToList();

            return bajoStock;
        }
        public async Task<bool> Eliminar(long id)
        {
            return await _dalc.Eliminar(id);
        }
    }
}
