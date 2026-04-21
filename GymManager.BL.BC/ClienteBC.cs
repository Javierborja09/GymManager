using GymManager.BL.BE;
using GymManager.DL.DALC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BC
{
    public class ClienteBC
    {
        private readonly ClienteDALC _clienteDALC;
        public ClienteBC(string connectionString)
        {
            _clienteDALC = new ClienteDALC(connectionString);
        }
        public async Task<List<ClienteBE>> ListarClientes(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length < 2)
            {
                return await _clienteDALC.ListarClientes(null);
            }
            return await _clienteDALC.ListarClientes(dni);
        }

        public async Task<List<ClienteBE>> ListarActivos()
        {
            var clientes = await _clienteDALC.ListarClientes(null);
            var estado = "Activo";
            var activos = clientes
                .Where(c => c.estado == estado)
                .OrderBy(c => c.nombre)
                .ToList();

            return activos;
        }


    }
}
