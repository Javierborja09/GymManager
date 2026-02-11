using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long usuario_id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string email { get; set; } = string.Empty;

        [Required]
        public string password_hash { get; set; } = string.Empty;

        public string rol { get; set; } = "Vendedor";

        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        public bool activo { get; set; } = true;
    }
}