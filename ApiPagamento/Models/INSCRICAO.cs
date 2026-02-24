using System;
using System.Collections.Generic;
namespace PagamentoApi.Models
{
    public partial class INSCRICAO
    {
        public int Id { get; protected set; }
        public int CDUOP { get; set; }
        public int CDPROGRAMA { get; set; }
        public int SQMATRIC { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public int? CDDESCONTO { get; set; }
        public int CDFONTEINF { get; set; }
        public int? CDFORMATO { get; set; }
        public int? CDPERFIL { get; set; }
        public short STINSCRI { get; set; }
        public DateTime? DTPREINSCR { get; set; }
        public DateTime? DTINSCRI { get; set; }
        public string LGINSCRI { get; set; }
        public DateTime? DTPRIVENCT { get; set; }
        public short NUCOBRANCA { get; set; }
        public short CDUOPINSC { get; set; }
        public DateTime DTSTATUS { get; set; }
        public TimeSpan HRSTATUS { get; set; }
        public string LGSTATUS { get; set; }
        public short CDUOPSTAT { get; set; }
        public string DSSTATUS { get; set; }
        public short? STCANCELAD { get; set; }
        public short? CDCATEGORI { get; set; }

        public DateTime DTFIMOCORR { get; set; }

        public virtual PROGOCORR PROGOCORR { get; set; }
        public virtual CLIENTELA CLIENTELA { get; set; }
    }
}