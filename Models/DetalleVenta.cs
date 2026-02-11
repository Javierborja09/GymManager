using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("DetalleVentas")]
    public class DetalleVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long detalle_id { get; set; }

        public long venta_id { get; set; }
        public long producto_id { get; set; }

        public int cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_unitario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal subtotal { get; private set; }

        [ForeignKey("venta_id")]
        public virtual Venta? Venta { get; set; }

        [ForeignKey("producto_id")]
        public virtual Producto? Producto { get; set; }
    }
}