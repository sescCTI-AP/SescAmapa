namespace PagamentoApi.Models
{
    public class PARAMATIV
    {
        public int CDMAPA { get; set; }
        public int CDREALIZAC { get; set; }
        //Cobrar multa
        public int VBCOBMULTA { get; set; }
        //Percentual da multa
        public decimal PERCMULTA { get; set; }
        //Dia inicio cobranca de multa
        public int DINIMULTA { get; set; }
        //Cobrar juros?
        public int VBCOBMORA { get; set; }
        //Percentual do juros
        public decimal PERCMORA { get; set; }
        //Dia inicio contagem juros
        public int DINIMORA { get; set; }
        //Dias calculo do juros
        public short? DCALMORA { get; set; }
        //0 - Juros Composto
        //1 - Juros Simples
        public short TPMORA { get; set; }
    }
}