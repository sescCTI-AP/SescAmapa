namespace SiteSesc.Models.ApiPagamento
{
    public class ClienteAdd
    {
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public string CDCLASSIF { get; set; }
        public short NUDV { get; set; }
        public string NUCGCCEI { get; set; }
        public short CDCATEGORI { get; set; }
        public Nullable<short> CDNIVEL { get; set; }
        public Nullable<int> SQTITULMAT { get; set; }
        public Nullable<int> CDUOTITUL { get; set; }
        public short STMATRIC { get; set; }
        public System.DateTime DTINSCRI { get; set; }
        public string CDMATRIANT { get; set; }
        public System.DateTime DTVENCTO { get; set; }
        public string NMCLIENTE { get; set; }
        public System.DateTime DTNASCIMEN { get; set; }
        public string NMPAI { get; set; }
        public string CDSEXO { get; set; }
        public string NMMAE { get; set; }
        public short CDESTCIVIL { get; set; }
        public short VBESTUDANT { get; set; }
        public Nullable<short> NUULTSERIE { get; set; }
        public string DSNATURAL { get; set; }
        public string DSNACIONAL { get; set; }
        public Nullable<short> NUDEPEND { get; set; }
        public string NUCTPS { get; set; }
        public Nullable<System.DateTime> DTADMISSAO { get; set; }
        public Nullable<System.DateTime> DTDEMISSAO { get; set; }
        public string NUREGGERAL { get; set; }
        public Nullable<decimal> VLRENDA { get; set; }
        public string NUCPF { get; set; }
        public string NUPISPASEP { get; set; }
        public string DSCARGO { get; set; }
        public Nullable<System.DateTime> DTEMIRG { get; set; }
        public string IDORGEMIRG { get; set; }
        public Nullable<short> DSPARENTSC { get; set; }
        public byte[] FOTO { get; set; }
        public System.DateTime DTATU { get; set; }
        public Nullable<short> STEMICART { get; set; } = 1;
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public double SMFIELDATU { get; set; } = 0.0;
        public string TEOBS { get; set; }
        public int NRVIACART { get; set; } = 1;
        public string PSWCLI { get; set; } = "123456";
        public Nullable<int> NUMCARTAO { get; set; }
        public string PSWCRIP { get; set; } = "012345";
        public Nullable<decimal> VLRENDAFAM { get; set; }
        public string NMSOCIAL { get; set; }
        public short SITUPROF { get; set; }
        public short TIPOIDENTIDADE { get; set; } = 0;
        public string COMPIDENTIDADE { get; set; }
        public short VBPCG { get; set; } = 0;
        public short VBEMANCIPADO { get; set; } = 0;
        public short VBPCD { get; set; } //0 - NAO | 1 - SIM
        public string IDNACIONAL { get; set; }
        public int STONLINE { get; set; } = 0;
        public int VBNOMEAFETIVO { get; set; } = 0;
        public string FOTO64 { get; set; }
        public string RAZAOSOCIAL { get; set; }
        public string CNAE { get; set; }
        public string CNAE2 { get; set; }
        public EnderecoAdd ENDERECO { get; set; }
    }

    public class EnderecoAdd
    {
        public int Id { get; protected set; }
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public short SQENDEREC { get; set; }
        public string DSLOGRADOU { get; set; }
        public string SIESTADO { get; set; }
        public string DSCOMPLEM { get; set; }
        public short CDMUNICIP { get; set; }
        public Nullable<int> NUIMOVEL { get; set; }
        public string DSBAIRRO { get; set; }
        public string NUCEP { get; set; }
        public short STPRINCIP { get; set; }
        public double SMFIELDATU { get; set; }
        public string DSMUNICIP { get; set; }
    }

}
