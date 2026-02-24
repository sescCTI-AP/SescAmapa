using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Pix
{
    public class PixRecarga
    {
        public int Cduop { get; set; }
        public int Sqmatric { get; set; }
        public int Cartao { get; set; }
        public int SqMoviment { get; set; }
        public PixCriar Pix { get; set; }
        public int Tipo { get; set; }

        public bool IsHorarioDeGeracaoPix()
        {
            return (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 30) ? false : true;

        }
    }
}