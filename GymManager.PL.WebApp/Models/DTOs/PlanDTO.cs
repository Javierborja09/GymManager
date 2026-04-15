namespace GymManager.Web.Models.DTOs
{
    public class PlanDTO
    {
        public long PlanId { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public int DuracionDias { get; set; }
        public decimal Precio { get; set; }
    }
}