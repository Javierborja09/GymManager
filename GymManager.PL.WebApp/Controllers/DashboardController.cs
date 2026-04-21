using GymManager.BL.BC;
using GymManager.Data;
using GymManager.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Security.Claims;

namespace GymManager.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ClienteBC _clienteBC;
        private readonly MatriculaBC _matriculaBC;
        private readonly ProductosBC _productosBC;
        private readonly MetasMensualesBC _metasBC;
        private readonly VentaBC _ventaBC;

        public DashboardController(ClienteBC clienteBC, MatriculaBC matriculaBC, ProductosBC productosBC, MetasMensualesBC metasBC, VentaBC ventaBC)
        {
            _clienteBC = clienteBC;
            _matriculaBC = matriculaBC;
            _productosBC = productosBC;
            _metasBC = metasBC;
            _ventaBC = ventaBC;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var mesActual = hoy.Month;
            var anioActual = hoy.Year;

            // 1. Obtener listas base desde las capas de negocio
            var clientesActivos = await _clienteBC.ListarActivos();
            var productosBajoStock = await _productosBC.ObtenerProductosBajoStock(5);
            var todasLasMatriculas = await _matriculaBC.ListarMatriculas();

            // 2. Obtener meta del mes con su progreso (Ya calculado en tu MetasMensualesBC)
            var metasAnio = await _metasBC.ListarMetasConProgreso(anioActual);
            var metaMesActual = metasAnio.FirstOrDefault(m => m.mes == mesActual);

            // 3. Poblar el DTO
            var dto = new DashboardDTO
            {
                // Conteos
                SociosActivos = clientesActivos.Count,
                BajoStock = productosBajoStock.Count,

                // Lógica para "Por Vencer": Matrículas que vencen en los próximos 7 días
                PorVencer = todasLasMatriculas.Count(m => m.fecha_fin >= hoy && m.fecha_fin <= hoy.AddDays(7)),

                MesNombre = hoy.ToString("MMMM").ToUpper(),

                // Recaudación de HOY (Filtrando sobre la lista de la capa de negocio)
                MembresiasHoy = await _matriculaBC.ObtenerRecaudacionHoy(),

                
                ProductosHoy = await _ventaBC.CalcularVentasHoy(),

                // Datos de la Meta (Usando el BE que ya trae el RecaudadoReal calculado)
                MetaMonto = metaMesActual?.objetivo_monto ?? 0,
                MetaRecaudado = metaMesActual?.RecaudadoReal ?? 0
            };

            return View(dto);
        }
    }
}