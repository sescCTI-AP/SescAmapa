using System.Runtime.Serialization;

namespace PagamentoApi.V2.Models.Pix
{
    [DataContract]
    public class PixHorario
    {
        [DataMember(Name = "calendario")]
        public string Solicitacao { get; set; }
        [DataMember(Name = "calendario")]
        public string Liquidacao { get; set; }
    }
}