using GymManager.BL.BE;
using GymManager.DL.DALC;

namespace GymManager.BL.BC
{
    public class ClienteBC
    {
        private readonly ClienteDALC _clienteDALC;

        public ClienteBC(string connectionString)
        {
            _clienteDALC = new ClienteDALC(connectionString);
        }

        public async Task<List<ClienteBE>> ListarClientes(string? dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length < 2)
                return await _clienteDALC.ListarClientes(null);

            return await _clienteDALC.ListarClientes(dni);
        }


        public async Task<List<ClienteBE>> ListarActivos()
        {
            var clientes = await _clienteDALC.ListarClientes(null);
            return clientes
                .Where(c => c.estado == "Activo")
                .OrderBy(c => c.nombre)
                .ToList();
        }

        public async Task<ClienteBE?> ObtenerPorId(long id)
        {
            if (id <= 0) return null;
            return await _clienteDALC.ObtenerPorId(id);
        }

        public async Task<(bool Ok, string? Error)> RegistrarCliente(ClienteBE cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.dni))
                return (false, "El DNI es obligatorio.");

            if (string.IsNullOrWhiteSpace(cliente.nombre))
                return (false, "El nombre es obligatorio.");

            await _clienteDALC.RegistrarCliente(cliente);
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> ActualizarCliente(ClienteBE cliente)
        {
            if (cliente.cliente_id <= 0)
                return (false, "Cliente no válido.");

            var existe = await _clienteDALC.ObtenerPorId(cliente.cliente_id);
            if (existe == null)
                return (false, "El cliente no existe.");

            await _clienteDALC.ActualizarCliente(cliente);
            return (true, null);
        }
    }
}