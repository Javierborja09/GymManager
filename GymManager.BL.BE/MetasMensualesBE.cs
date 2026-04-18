using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BE
{
    public class MetasMensualesBE
    {
        [Key]
        public int meta_id { get; set; }
        public int mes { get; set; }
        public int anio { get; set; }
        public decimal objetivo_monto { get; set; }
        public string? descripcion { get; set; }

        [NotMapped] // Solo para la vista
        public decimal RecaudadoReal { get; set; }
    }
}
