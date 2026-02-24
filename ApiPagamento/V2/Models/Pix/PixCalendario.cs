using System;
using System.Runtime.Serialization;
namespace PagamentoApi.V2.Models.Pix
{
    [DataContract]
    public class PixCalendario
    {
        /// <summary>
        /// Timestamp que indica o momento em que foi criada a cobrança. Respeita o formato definido na RFC 3339.
        /// </summary>
        /// <value></value>
        [DataMember(Name = "criacao")]
        public DateTime Criacao { get; set; }
        /// <summary>
        /// Tempo de vida da cobrança, especificado em segundos a partir da data de criação,
        ///  para que o pagamento da cobrança possa ser realizado (Calendario.criacao). 
        /// Se não for informado, assume-se a duração de 86400 segundos, que corresponde a 24 horas.
        /// Exemplo: 3600 (indica validade de 1 hora).
        /// </summary>
        /// <value></value>
        [DataMember(Name = "expiracao")]
        public int Expiracao { get; set; }
    }
}