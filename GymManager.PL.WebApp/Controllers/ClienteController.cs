using GymManager.BL.BC;
using GymManager.BL.BE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManager.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ClienteBC _clienteBC;

        public ClienteController(IConfiguration configuration)
        {
            _clienteBC = new ClienteBC(
                configuration.GetConnectionString("DefaultConnection")!);
        }

        // GET: /Cliente
        public async Task<IActionResult> Index(string? buscarDni)
        {
            var clientes = await _clienteBC.ListarClientes(buscarDni);

            ViewData["FiltroDni"] = buscarDni;
            return View(clientes);
        }

        // GET: /Cliente/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var cliente = await _clienteBC.ObtenerPorId(id.Value);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: /Cliente/Create
        public IActionResult Create() => View();

        // POST: /Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteBE cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            var (ok, error) = await _clienteBC.RegistrarCliente(cliente);
            if (!ok)
            {
                ModelState.AddModelError("", error!);
                return View(cliente);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Cliente/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var cliente = await _clienteBC.ObtenerPorId(id.Value);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: /Cliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ClienteBE cliente)
        {
            if (id != cliente.cliente_id) return NotFound();
            if (!ModelState.IsValid) return View(cliente);

            var (ok, error) = await _clienteBC.ActualizarCliente(cliente);
            if (!ok)
            {
                ModelState.AddModelError("", error!);
                return View(cliente);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}