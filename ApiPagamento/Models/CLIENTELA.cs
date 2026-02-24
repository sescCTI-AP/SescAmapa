using System;
using System.Collections.Generic;
namespace PagamentoApi.Models
{

    public partial class CLIENTELA
    {
        public int Id { get; protected set; }
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
        public Nullable<short> STEMICART { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public double SMFIELDATU { get; set; }
        public string TEOBS { get; set; }
        public int NRVIACART { get; set; }
        public string PSWCLI { get; set; }
        public Nullable<int> NUMCARTAO { get; set; }
        public string PSWCRIP { get; set; }
        public Nullable<decimal> VLRENDAFAM { get; set; }
        public string NMSOCIAL { get; set; }
        public short SITUPROF { get; set; }
        public short TIPOIDENTIDADE { get; set; }
        public string COMPIDENTIDADE { get; set; }
        public short VBPCG { get; set; }
        public short VBEMANCIPADO { get; set; }
        public short VBPCD { get; set; }
        public string IDNACIONAL { get; set; }
        public virtual ICollection<INSCRICAO> INSCRICAO { get; set; }
        public virtual ICollection<COBRANCA> COBRANCA { get; set; }
        public virtual ICollection<ENDERECOS> ENDERECOS { get; set; }
        public virtual RESPCLI RESPCLI { get; set; }
        public virtual UOP UOP { get; set; }

    }
}