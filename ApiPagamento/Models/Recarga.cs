using PagamentoApi.Models.Cielo;
using System;

namespace PagamentoApi.Models
{
    public class Recarga
    {
        public int NumCartao { get; set; }
        public decimal Valor { get; set; }
        public Cliente Customer { get; set; }
        public Payment Payment { get; set; }

        public bool IsHorarioDeGeracaoRecarga()
        {
            return (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 30) ? false : true;

        }
    }
}