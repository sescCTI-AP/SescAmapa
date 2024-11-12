using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.Relatorio
{
    [NotMapped]
    public class TransacaoRealizada
    {
        public int id { get; set; }
        public string idclasse { get; set; }
        public string cdelement { get; set; }
        public int sqcobranc { get; set; }
        public string merchantorder { get; set; }
        public string cardnumber { get; set; }
        public string brand { get; set; }
        public string proofofsale { get; set; }
        public string tid { get; set; }
        public string authorizationcode { get; set; }
        public string paymentid { get; set; }
        public string tipo { get; set; }
        public decimal valor { get; set; }
        public int parcelas { get; set; }
        public DateTime dtoperacao { get;set;}
        public int caixa { get; set; }
        public int cdpessoa { get; set; }
        public int? sqdepret { get; set; }
        public int? cancelado { get; set; }
    }
}