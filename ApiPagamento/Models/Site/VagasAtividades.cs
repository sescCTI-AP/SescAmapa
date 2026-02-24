using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Site
{
    public class VagasAtividades
    {
        public int VAGASTOTAIS { get; set; }
        public int VAGASOCUPADAS { get; set; }

        [NotMapped]
        public double PercVagasTotais => VAGASOCUPADAS * 100 / VAGASTOTAIS;

    }
}
