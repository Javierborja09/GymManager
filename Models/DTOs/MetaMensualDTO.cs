namespace GymManager.Web.Models.DTOs
{
    public class MetaMensualDTO
    {
        public int MetaId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string MesNombre { get; set; } = string.Empty;
        public decimal ObjetivoMonto { get; set; }
        public decimal RecaudadoReal { get; set; }
        public string? Descripcion { get; set; }
        public int PorcentajeAlcanzado => ObjetivoMonto > 0
            ? (int)((RecaudadoReal / ObjetivoMonto) * 100)
            : 0;
    }
}