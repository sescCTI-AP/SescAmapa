using System.Text.Json.Serialization;
using System;

namespace PagamentoApi.Models.Responses
{
    public class TransacaoPixResponse
    {
        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("chave")]
        public string Chave { get; set; } = string.Empty;

        [JsonPropertyName("txid")]
        public string Txid { get; set; } = string.Empty;

        [JsonPropertyName("pixCopiaECola")]
        public string PixCopiaECola { get; set; } = string.Empty;

        [JsonPropertyName("criacao")]
        public DateTime Criacao { get; set; }

        [JsonPropertyName("dataCriacao")]
        public string DataCriacao { get; set; } = string.Empty;

        [JsonPropertyName("expiracao")]
        public int Expiracao { get; set; }

        [JsonPropertyName("dataExpiracao")]
        public string DataExpiracao { get; set; } = string.Empty;

        [JsonPropertyName("valorOriginal")]
        public decimal ValorOriginal { get; set; }

        [JsonPropertyName("situacao")]
        public string Situacao { get; set; } = string.Empty;

        [JsonPropertyName("solicitacaoPagador")]
        public string SolicitacaoPagador { get; set; } = string.Empty;

        [JsonPropertyName("devedor")]
        public TransacaoPixResponseDevedor Devedor { get; set; }

        [JsonPropertyName("pagador")]
        public TransacaoPixResponsePagador Pagador { get; set; }

    }
}
