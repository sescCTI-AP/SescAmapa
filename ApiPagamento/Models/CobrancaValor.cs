using System;

namespace PagamentoApi.Models
{
    public class CobrancaValor
    {
        public CobrancaValor() { }
        public CobrancaValor(string iDCLASSE, string cDELEMENT, int sQCOBRANCA, decimal valorOriginal,
            decimal valorRecebido, decimal jurosMora, decimal multa, decimal descontoConcedido)
        {
            IDCLASSE = iDCLASSE;
            CDELEMENT = cDELEMENT;
            SQCOBRANCA = sQCOBRANCA;
            ValorOriginal = valorOriginal;
            ValorRecebido = valorRecebido;
            JurosMora = jurosMora;
            Multa = multa;
            DescontoConcedido = descontoConcedido;
        }

        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANCA { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal JurosMora { get; set; }
        public decimal Multa { get; set; }
        public decimal OutrosRecebimentos { get; set; }
        public decimal Acrescimo { get; set; }
        public decimal DescontoConcedido { get; set; }
        public string Atividade { get; set; }
        public DateTime Vencimento { get; set; }
    }
}