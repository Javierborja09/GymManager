using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.BL.BE
{
    public class MatriculaBE
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
        public virtual ClienteBE? Cliente { get; set; }

        [ForeignKey("plan_id")]
        public virtual PlanBE? Plan { get; set; }
    }
}
