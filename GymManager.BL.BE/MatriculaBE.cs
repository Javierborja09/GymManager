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
        public long matricula_id { get; set; }

        public long cliente_id { get; set; }
        public long plan_id { get; set; }

        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public decimal monto_pagado { get; set; }
        public ClienteBE Cliente { get; set; } = new ClienteBE();
        public PlanBE Plan { get; set; } = new PlanBE();
    }
}
