using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Pix
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