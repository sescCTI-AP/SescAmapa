using System;

namespace PagamentoApi.Models.Boleto
{
    public class SescTO_NossoNumero
    {
        public int ID { get; set; }
        public long NUMERO { get; set; }
        public int PRODUCAO { get; set; }
        public DateTime DATA_ULTIMA_ATUALIZACAO { get; set; }
    }
}