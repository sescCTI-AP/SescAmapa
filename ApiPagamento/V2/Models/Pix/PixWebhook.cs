 
using Newtonsoft.Json;
using System;

namespace PagamentoApi.V2.Models.Pix
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
        

        [JsonProperty("pagador")]
        public Pagador Pagador { get; set; }
    }
    public class Pagador
    {
        [JsonProperty("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [JsonProperty("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonProperty("nome")]
        public string Nome { get; set; } = string.Empty;

    }

}
