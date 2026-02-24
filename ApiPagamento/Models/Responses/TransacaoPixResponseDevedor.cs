using System.Text.Json.Serialization;

namespace PagamentoApi.Models.Responses
{
    public class TransacaoPixResponseDevedor
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; } = string.Empty;

    }
}
