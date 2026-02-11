namespace GymManager.Models.DTOs
{
    public class MatriculaDTO
    {
        public string ClienteNombre { get; set; } = string.Empty;
        public string PlanNombre { get; set; } = string.Empty;
        public DateTime fecha_fin { get; set; }
        public bool EstaVencida => DateTime.Now > fecha_fin;
    }
}