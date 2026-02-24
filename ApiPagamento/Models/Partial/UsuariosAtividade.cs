using System;
using System.Collections.Generic;

namespace PagamentoApi.Models.Partial
{
    public class UsuariosAtividade
    {
        public string NMCLIENTE { get; set; }
        public DateTime DTNASCIMEN { get; set; }
        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public int CDFORMATO { get; set; }
        public int CDFONTEINF { get; set; }
        public int CDPERFIL { get; set; }
        public int STINSCRI { get; set; }
        public DateTime DTINSCRI { get; set; }
        public int LGINSCRI { get; set; }
        public int NUCOBRANCA { get; set; }
        public int CDUOPINSC { get; set; }
        public DateTime DTSTATUS { get; set; }
        public TimeSpan HRSTATUS { get; set; }
        public int LGSTATUS { get; set; }
        public int CDUOPSTAT { get; set; }
        public string DSCATEGORI { get; set; }
        public string NMUOP { get; set; }
        public int NUCOBEXC { get; set; }
        public int NUCOBREST { get; set; }
        public List<CONTATOS> CONTATOS { get; set; }
    }
}