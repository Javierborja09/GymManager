namespace GymManager.Models.DTOs
{
    public class VentaRequestDTO
    {
        public long? cliente_id { get; set; }
        public long usuario_id { get; set; }
        public List<DetalleItemDTO> Items { get; set; } = new();
    }

    public class DetalleItemDTO
    {
        public long producto_id { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
    }
}