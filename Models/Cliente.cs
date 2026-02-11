using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long cliente_id { get; set; }

        [Required]
        [StringLength(20)]
        public string dni { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string apellido { get; set; } = string.Empty;

        public string? telefono { get; set; }
        public string? email { get; set; }
        public DateTime fecha_registro { get; set; } = DateTime.Now;
        public string estado { get; set; } = "Activo";
    }
}