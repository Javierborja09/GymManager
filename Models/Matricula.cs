using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("Matriculas")]
    public class Matricula
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long matricula_id { get; set; }

        public long cliente_id { get; set; }
        public long plan_id { get; set; }

        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal monto_pagado { get; set; }

        [ForeignKey("cliente_id")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("plan_id")]
        public virtual Plan? Plan { get; set; }
    }
}