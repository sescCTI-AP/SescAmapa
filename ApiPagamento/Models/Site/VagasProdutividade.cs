using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Site
{
    public class VagasProdutividade
    {
        public int VAGASTOTAIS { get; set; }
        public int VAGASOCUPADAS { get; set; }

        [NotMapped]
        public double OCUPACAO => VAGASOCUPADAS * 100 / VAGASTOTAIS;
    }
}
