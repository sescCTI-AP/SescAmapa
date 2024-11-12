using SiteSesc.Models.DB2;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ApiPagamento.Relatorios
{
    public class InscritosPorUnidade
    {
        public int cdprograma { get;set;}
        public int cdconfig { get;set;}
        public int sqocorrenc { get;set;}
        public string dsusuario { get;set;}
        public DateTime? dtlinscri { get;set;}
        public DateTime? dtuinscri { get;set;}
        public DateTime? dtiniocorr { get;set;}
        public DateTime? dtfimocorr { get;set;}
        public int? vbinscruop { get;set;}
        public string idusrresp { get;set;}
        public int nuvagas { get;set;}
        public int nuvagasocp { get;set;}
        public int numinvocup { get;set;}
        public int vbocorrapr { get;set;}
        public int cduopcad { get;set;}
        public DateTime? dtaprov { get;set;}
        public DateTime? dtatu { get;set; }
        public string lgatu { get;set; }
        public int duraula { get;set; }
        public decimal? idademin { get;set; }
        public decimal? idademax { get;set; }
        public int? vbcancela { get;set; }
        public string aamoda { get;set; }
        public List<UsuarioInscrito> inscritos { get; set; }

        [NotMapped]
        public string cdelement => $"{cdprograma.ToString().PadLeft(8, '0')}{cdconfig.ToString().PadLeft(8, '0')}{sqocorrenc.ToString().PadLeft(8, '0')}";

        public static List<InscritosPorUnidade> ToInscritosUnidades(List<PROGRAMAOCORRENCIA> prog)
        {
            var listaProgramas = new List<InscritosPorUnidade>();
            if (prog.Any())
            {
                foreach (var p in prog)
                {
                    listaProgramas.Add(new InscritosPorUnidade
                    {
                        cdprograma = p.CDPROGRAMA,
                        cdconfig = p.CDCONFIG,
                        sqocorrenc = p.SQOCORRENC,
                        dsusuario = p.DSUSUARIO,
                        nuvagas = p.NUVAGAS,
                        nuvagasocp = p.NUVAGASOCP,
                        numinvocup = 0,
                        vbocorrapr = 0,
                        cduopcad = p.CDUOPCAD,
                        duraula = 0,
                        aamoda = p.AAMODA.ToString(),
                        inscritos = null
                    });
                }
            }
            return listaProgramas;
        }
    }

    public class UsuarioInscrito
    {
        //public string nmcliente { get; set; }
        //public DateTime? dtnascimen { get; set; }
        //public int sqmatric { get; set; }
        //public int cdprograma { get; set; }
        //public int cdconfig { get; set; }
        //public int sqocorrenc { get; set; }
        //public int cdformato { get; set; }
        //public int cdfonteinf { get; set; }
        //public int cdperfil { get; set; }
        public int stinscri { get; set; }
        //public DateTime? dtinscri { get; set; }
        //public int? lginscri { get; set; }
        //public int nucobranca { get; set; }
        //public int cduopinsc { get; set; }
        //public DateTime dtstatus { get; set; }
        //public int lgstatus { get; set; }
        //public int cduopstat { get; set; }
        public string dscategori { get; set; }
    }
}
