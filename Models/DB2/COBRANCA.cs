namespace SiteSesc.Models.DB2
{
    public class COBRANCA
    {
        public int Id { get; protected set; }
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANCA { get; set; }
        public int CDUOPCOB { get; set; }
        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public string DSCOBRANCA { get; set; }
        public short RFCOBRANCA { get; set; }
        public decimal VLCOBRADO { get; set; }
        public DateTime DTVENCTO { get; set; }
        public DateTime DTEMISSAO { get; set; }
        public short STRECEBIDO { get; set; }
        public Nullable<short> TPCOBRANCA { get; set; }
        public Nullable<decimal> PCJUROS { get; set; }
        public DateTime DTATU { get; set; }
        public double SMFIELDATU { get; set; }
        public string LGATU { get; set; }
        public string VLCARACTE1 { get; set; }
        public string VLCARACTE2 { get; set; }
        public Nullable<short> DDCOBJUROS { get; set; }
        public Nullable<short> DDINIJUROS { get; set; }
        public Nullable<decimal> PCMULTA { get; set; }
        public string DSCANCELAM { get; set; }
        public int CDUOPREC { get; set; }
        public string NMESTACAO { get; set; }
        public Nullable<int> CDCANCELA { get; set; }
        public short TPMORA { get; set; }
        public string LGCANCEL { get; set; }
        public Nullable<int> IDCOBRANCA { get; set; }

        public COBRANCA()
        {
        }

        public COBRANCA(string iDCLASSE, string cDELEMENT, int sQCOBRANCA, short sTRECEBIDO)
        {
            IDCLASSE = iDCLASSE;
            CDELEMENT = cDELEMENT;
            SQCOBRANCA = sQCOBRANCA;
            STRECEBIDO = sTRECEBIDO;
        }
    }
}
