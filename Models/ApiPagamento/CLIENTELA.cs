using SiteSesc.Models.DB2;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public class CLIENTELA
    {
        public int Id { get; protected set; }
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public string CDCLASSIF { get; set; }
        public short NUDV { get; set; }
        public string NUCGCCEI { get; set; }
        public short CDCATEGORI { get; set; }
        public short? CDNIVEL { get; set; }
        public int? SQTITULMAT { get; set; }
        public int? CDUOTITUL { get; set; }
        public short STMATRIC { get; set; }
        public DateTime DTINSCRI { get; set; }
        public string CDMATRIANT { get; set; }
        public DateTime DTVENCTO { get; set; }
        public string NMCLIENTE { get; set; }
        public string NMSOCIAL { get; set; }
        public DateTime DTNASCIMEN { get; set; }
        public string NMPAI { get; set; }
        public string CDSEXO { get; set; }
        public string NMMAE { get; set; }
        public short CDESTCIVIL { get; set; }
        public short VBESTUDANT { get; set; }
        public short? NUULTSERIE { get; set; }
        public string DSNATURAL { get; set; }
        public string DSNACIONAL { get; set; }
        public short? NUDEPEND { get; set; }
        public string NUCTPS { get; set; }
        public DateTime? DTADMISSAO { get; set; }
        public DateTime? DTDEMISSAO { get; set; }
        public string NUREGGERAL { get; set; }
        public decimal? VLRENDA { get; set; }
        public string NUCPF { get; set; }
        public string NUPISPASEP { get; set; }
        public string DSCARGO { get; set; }
        public DateTime? DTEMIRG { get; set; }
        public string IDORGEMIRG { get; set; }
        public short? DSPARENTSC { get; set; }
        public byte[] FOTO { get; set; }
        public DateTime DTATU { get; set; }
        public short? STEMICART { get; set; }
        //public DateTime HRATU { get; set; }
        public string LGATU { get; set; }
        public double SMFIELDATU { get; set; }
        public string TEOBS { get; set; }
        public int NRVIACART { get; set; }
        public string PSWCLI { get; set; }
        public int? NUMCARTAO { get; set; }
        public string PSWCRIP { get; set; }
        public decimal? VLRENDAFAM { get; set; }
        public short SITUPROF { get; set; }
        public short TIPOIDENTIDADE { get; set; }
        public string COMPIDENTIDADE { get; set; }
        public short VBPCG { get; set; }
        public short VBEMANCIPADO { get; set; }
        public short VBPCD { get; set; }
        public string IDNACIONAL { get; set; }

        public virtual ICollection<INSCRICAO> INSCRICAO { get; set; }
        public virtual ICollection<ENDERECODB2> ENDERECOS { get; set; }

        public virtual ICollection<COBRANCA> COBRANCA { get; set; }
        public virtual RESPCLI RESPCLI { get; set; }
        public virtual UOP UOP { get; set; }

        [NotMapped]
        public string Matricula => $"{CDUOP.ToString().PadLeft(4, '0')}-{SQMATRIC.ToString().PadLeft(6, '0')}-{NUDV}";

        [NotMapped]
        public string FotoCliente
        {
            get
            {
                if (FOTO != null)
                {
                    var imgBase64Dados = Convert.ToBase64String(FOTO);
                    var imagemCliente = $"data:image/png;base64,{imgBase64Dados}";
                    return imagemCliente;
                }
                return null;
            }
        }
    }
}
