using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ApiPagamento.Relatorios
{
    public class ProgramaAno
    {
        public string Mes { get; set; }
        public int Ano { get; set; }
        public int Status { get; set; }
        public int Contagem { get; set; }
    }
}
