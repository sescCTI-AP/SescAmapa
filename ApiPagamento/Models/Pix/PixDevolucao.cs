using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Pix
{
    [DataContract]
    public class PixDevolucao
    {
        [DataMember(Name = "id")]
        public string id { get; set; }
        [DataMember(Name = "rtrId")]
        public string rtrId { get; set; }
        [DataMember(Name = "valor")]
        public decimal Valor { get; set; }
        [DataMember(Name = "horario")]
        public PixHorario Horario { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "motivo")]
        public string Motivo { get; set; }

    }
}