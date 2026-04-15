namespace GymManager.Models.DTOs
{
    public class VentaDTO
    {
        public long VentaId { get; set; }
        public DateTime Fecha { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<VentaDetalleDTO> Productos { get; set; } = new();
    }

    public class VentaDetalleDTO
    {
        public long ProductoId { get; set; }

        public string? ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
    public class VentaItemDTO

    {
        public long producto_id { get; set; }
        public int cantidad { get; set; }

    }
}