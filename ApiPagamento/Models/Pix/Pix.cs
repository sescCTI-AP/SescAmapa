using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Pix
{
    [DataContract]
    public class Pix
    {
        [DataMember(Name = "endToEndId")]
        public string EndToEndId { get; set; }

        [DataMember(Name = "txid")]
        public string TxId { get; set; }

        [DataMember(Name = "valor")]
        public decimal Valor { get; set; }

        [DataMember(Name = "horario")]
        public DateTime Horario { get; set; }

        [DataMember(Name = "pagador")]
        public PixDevedor Pagador { get; set; }

        [DataMember(Name = "infoPagador")]
        public string InfoPagador { get; set; }

        [DataMember(Name = "devolucoes")]
        public PixDevolucao[] Devolucoes { get; set; }
    }
}