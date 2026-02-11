using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Asistencias")]
    public class Asistencia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long asistencia_id { get; set; }

        public long cliente_id { get; set; }

        public DateTime fecha_hora { get; set; } = DateTime.Now;

        [ForeignKey("cliente_id")]
        public virtual Cliente? Cliente { get; set; }
    }
}