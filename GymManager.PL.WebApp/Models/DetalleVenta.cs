using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    public class DetalleVenta
    {
        [Key]
        public long detalle_id { get; set; }
        public long venta_id { get; set; }
        public long producto_id { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; } 

        [ForeignKey("producto_id")]
        public virtual Producto? Producto { get; set; }
    }
}