namespace GymManager.Web.Models.DTOs
{
    public class DashboardDTO
    {
        // Estadísticas básicas
        public int SociosActivos { get; set; }
        public int BajoStock { get; set; }
        public int PorVencer { get; set; }

        // Recaudación Diaria
        public decimal MembresiasHoy { get; set; }
        public decimal ProductosHoy { get; set; }
        public decimal TotalRecaudadoHoy => MembresiasHoy + ProductosHoy;

        // Meta Mensual
        public string MesNombre { get; set; }
        public decimal MetaMonto { get; set; }
        public decimal MetaRecaudado { get; set; }
        public int MetaPorcentaje => MetaMonto > 0 ? (int)((MetaRecaudado / MetaMonto) * 100) : 0;
    }
}