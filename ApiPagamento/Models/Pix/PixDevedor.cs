using System.Runtime.Serialization;
namespace PagamentoApi.Models.Pix
{
    [DataContract]
    public class PixDevedor
    {
        [DataMember(Name = "cpf")]
        public string Cpf { get; set; }
        [DataMember(Name = "cnpj")]
        public string Cnpj { get; set; }
        [DataMember(Name = "nome")]
        public string Nome { get; set; }
    }
}