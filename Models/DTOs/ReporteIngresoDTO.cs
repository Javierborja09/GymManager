using System.ComponentModel.DataAnnotations;

namespace GymManager.Models.DTOs
{
    public class ReporteIngresoDTO
    {
        public string Tipo { get; set; } = string.Empty;

        public decimal Total { get; set; }
    }
}