using PagamentoApi.Models.Cielo;

namespace PagamentoApi.Models
{
    public class RecargaAvulsa
    {
        public int NumCartao { get; set; }
        public decimal Valor { get; set; }
        public int CdMoeda { get; set; }
        public bool IsTef { get; set; }
    }
}