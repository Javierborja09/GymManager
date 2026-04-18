using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BE
{
    public class VentasBE
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
        public virtual ClienteBE? Cliente { get; set; }

        [ForeignKey("usuario_id")]
        public virtual UsuarioBE? Usuario { get; set; }
        public virtual ICollection<DetalleVentaBE> DetalleVentas { get; set; } = new List<DetalleVentaBE>();
    }
}
