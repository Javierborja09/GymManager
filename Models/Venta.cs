using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long venta_id { get; set; }

        public long? cliente_id { get; set; }
        public long usuario_id { get; set; }

        public DateTime fecha_venta { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal total_venta { get; set; }

        [ForeignKey("cliente_id")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("usuario_id")]
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
    }
}