using NuGet.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public class CobrancaAtualizada
    {
        public string idclasse { get; set; }
        public string cdelement { get; set; }
        public int sqcobranca { get; set; }
        public decimal valorOriginal { get; set; }
        public decimal valorRecebido { get; set; }
        public decimal jurosMora { get; set; }
        public decimal multa { get; set; }
        public int outrosRecebimentos { get; set; }
        public decimal acrescimo { get; set; }
        public decimal descontoConcedido { get; set; }
        public string atividade { get; set; }
        public DateTime vencimento { get; set; }
        public int? strecebido { get; set; } = 0;

        public string Status
        {
            get{
                switch (strecebido)
                {
                    case 0:
                        return "Pendente";
                    case 1:
                        return "Pago";
                    case 3:
                        return "Cancelado";
                    default:
                        return "";
                }
            }

        }
    }
}
