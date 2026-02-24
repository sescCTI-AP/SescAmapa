using System.Text.Json.Serialization;

namespace PagamentoApi.Models.Responses
{
    public class TransacaoPixResponsePagador
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

    }
}
