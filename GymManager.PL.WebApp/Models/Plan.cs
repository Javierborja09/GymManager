using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Planes")]
    public class Plan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long plan_id { get; set; }

        [Required]
        [StringLength(50)]
        public string nombre_plan { get; set; } = string.Empty;

        public int duracion_dias { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal precio { get; set; }
    }
}