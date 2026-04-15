namespace GymManager.Models.DTOs
{
    public class MatriculaDTO
    {
        public long matricula_id { get; set; }
        public string SocioNombre { get; set; } = string.Empty;
        public string SocioDni { get; set; } = string.Empty;
        public string PlanNombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoPagado { get; set; }
        public int DiasRestantes => (FechaFin - DateTime.Now).Days;
    }
}