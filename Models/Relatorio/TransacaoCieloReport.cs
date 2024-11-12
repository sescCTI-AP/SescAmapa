using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Relatorio
{
    [NotMapped]
    public class TransacaoCieloReport
    {
        public TransacaoRealizada transacaoDb2 { get; set; }
        public PaymentValidacao transacaoValidacao { get; set; }
    }
}
