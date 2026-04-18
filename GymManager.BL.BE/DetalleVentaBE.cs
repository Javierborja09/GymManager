using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BE
{
    public class DetalleVentaBE
    {
        [Key]
        public long detalle_id { get; set; }
        public long venta_id { get; set; }
        public long producto_id { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }

        [ForeignKey("producto_id")]
        public virtual ProductoBE? Producto { get; set; }
    }
}
