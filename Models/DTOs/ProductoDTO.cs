namespace GymManager.Models.DTOs
{
    public class ProductoDTO
    {
        public long ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public int StockActual { get; set; }
        public string? Categoria { get; set; }
        public bool EsStockBajo => StockActual <= 5;
    }
}