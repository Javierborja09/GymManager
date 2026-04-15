using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long producto_id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_venta { get; set; }

        public int stock_actual { get; set; } = 0;
        public string? categoria { get; set; }
    }
}