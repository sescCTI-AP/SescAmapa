using System.Runtime.Serialization;
using System.Xml.Linq;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PagamentoApi.Models.Pix
{

    public class PixRecebe
    {
        [JsonProperty("pix")]
        public PixWeb[] Pix { get; set; }
    }


    public class PixWeb
    {
        [JsonProperty("endToEndId")]
        public string EndToEndId { get; set; }

        [JsonProperty("txid")]
        public string TxId { get; set; }

        [JsonProperty("valor")]
        public string Valor { get; set; }

        [JsonProperty("chave")]
        public string Chave { get; set; }

        [JsonProperty("horario")]
        public DateTime Horario { get; set; }

        [JsonProperty("infoPagador")]
        public string InfoPagador { get; set; }
    }
}
