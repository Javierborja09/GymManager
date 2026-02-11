using System.Diagnostics;
using GymManager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Importante

namespace GymManager.Web.Controllers
{
    [Authorize] // Asegura que solo usuarios autenticados entren al panel
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Dashboard/Error404")]
        public IActionResult Error404() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}