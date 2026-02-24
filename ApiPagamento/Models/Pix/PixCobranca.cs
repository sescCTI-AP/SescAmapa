using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Pix
{
    public class PixCobranca
    {
        public int Cduop { get; set; }
        public int Sqmatric { get; set; }
        public string CdElement { get; set; }
        public int SqCobranca { get; set; }
        public string IdClasse { get; set; }
        public decimal Valor { get; set; }
        public decimal Juros { get; set; }
        public decimal Multa { get; set; }
        public decimal Desconto { get; set; }
        public PixCriar Pix { get; set; }
        public int Tipo { get; set; }
    }
}