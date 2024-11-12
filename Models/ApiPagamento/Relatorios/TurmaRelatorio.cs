using SiteSesc.Models.ApiPagamento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ApiPagamento.Relatorios
{
    public class TurmaRelatorio
    {
        public int IdUop { get; set; }
        public string NomeUnidade { get; set; }
        public List<Programa> programas { get; set; }

        [NotMapped]
        public int NumeroTurmas => programas.Select(a => a.NumeroTurmas).Sum();

        [NotMapped]
        public int Capacidade => programas.Select(a => a.Capacidade).Sum();

        [NotMapped]
        public int VagasOcupadas => programas.Select(a => a.VagasOcupadas).Sum();

        [NotMapped]
        public int PublicoAlvo => programas.Select(a => a.PublicoAlvo).Sum();

        [NotMapped]
        public int PublicoGeral => programas.Select(a => a.PublicoGeral).Sum();

        [NotMapped]
        public decimal PorcentagemOcupacao => OcupacaoTotal == 0 ? 0 : OcupacaoTotal * 100 / Capacidade;

        [NotMapped]
        public decimal PorcentagemVagasDisponiveis => OcupacaoTotal == 0 ? 100 : 100 - PorcentagemOcupacao;

        [NotMapped]
        public decimal PorcentagemPublicoAlvo => OcupacaoTotal == 0 ? 0 : PublicoAlvo * 100 / OcupacaoTotal;

        [NotMapped]
        public decimal PorcentagemPublicoGeral => OcupacaoTotal == 0 ? 0 : PublicoGeral * 100 / OcupacaoTotal;

        [NotMapped]
        public int OcupacaoTotal => PublicoAlvo + PublicoGeral;

        [NotMapped]
        public int VagasDisponiveis => Capacidade - OcupacaoTotal;
    }

    public class Programa
    {
        public int IdArea { get; set; }
        public string NomePrograma { get; set; }

        public List<Modalidade> Modalidades { get; set; }

        [NotMapped]
        public int NumeroTurmas => Modalidades.Select(a => a.NumeroTurmas).Sum();

        [NotMapped]
        public int Capacidade => Modalidades.Select(a => a.Capacidade).Sum();

        [NotMapped]
        public int VagasOcupadas => Modalidades.Select(a => a.VagasOcupadas).Sum();

        [NotMapped]
        public int PublicoAlvo => Modalidades.Select(a => a.PublicoAlvo).Sum();

        [NotMapped]
        public int PublicoGeral => Modalidades.Select(a => a.PublicoGeral).Sum();

        [NotMapped]
        public decimal PorcentagemOcupacao => OcupacaoTotal == 0 ? 0 : OcupacaoTotal * 100 / Capacidade;

        [NotMapped]
        public decimal PorcentagemVagasDisponiveis => OcupacaoTotal == 0 ? 100 : 100 - PorcentagemOcupacao;

        [NotMapped]
        public decimal PorcentagemPublicoAlvo => OcupacaoTotal == 0 ? 0 : PublicoAlvo * 100 / OcupacaoTotal;

        [NotMapped]
        public decimal PorcentagemPublicoGeral => OcupacaoTotal == 0 ? 0 : PublicoGeral * 100 / OcupacaoTotal;

        [NotMapped]
        public int OcupacaoTotal => PublicoAlvo + PublicoGeral;

        [NotMapped]
        public int VagasDisponiveis => Capacidade - OcupacaoTotal;
    }

    public class Modalidade
    {
        public int IdSubArea { get; set; }
        public string NomeModalidade { get; set; }

        public List<Atividade> Atividades { get; set; }

        [NotMapped]
        public int NumeroTurmas => Atividades.Select(a => a.NumeroTurmas).Sum();

        [NotMapped]
        public int Capacidade => Atividades.Select(a => a.Capacidade).Sum();       

        [NotMapped]
        public int VagasOcupadas => Atividades.Select(a => a.VagasOcupadas).Sum();

        [NotMapped]
        public int PublicoAlvo => Atividades.Select(a => a.PublicoAlvo).Sum();

        [NotMapped]
        public int PublicoGeral => Atividades.Select(a => a.PublicoGeral).Sum();

        [NotMapped]
        public decimal PorcentagemOcupacao => OcupacaoTotal == 0 ? 0 : OcupacaoTotal * 100 / Capacidade;

        [NotMapped]
        public decimal PorcentagemVagasDisponiveis => OcupacaoTotal == 0 ? 100 : 100 - PorcentagemOcupacao;

        [NotMapped]
        public decimal PorcentagemPublicoAlvo => OcupacaoTotal == 0 ? 0 : PublicoAlvo * 100 / OcupacaoTotal;

        [NotMapped]
        public decimal PorcentagemPublicoGeral => OcupacaoTotal == 0 ? 0 : PublicoGeral * 100 / OcupacaoTotal;

        [NotMapped]
        public int OcupacaoTotal => PublicoAlvo + PublicoGeral;

        [NotMapped]
        public int VagasDisponiveis => Capacidade - OcupacaoTotal;
    }

    public class Atividade
    {
        public string CdElement { get; set; }
        public string NomeAtividade { get; set; }
        public int NumeroTurmas { get; set; }
        public int Capacidade { get; set; }
        public int VagasOcupadas { get; set; }
        public int PublicoAlvo { get; set; }
        public int PublicoGeral { get; set; }

        [NotMapped]
        public decimal PorcentagemOcupacao => OcupacaoTotal * 100 / Capacidade;

        [NotMapped]
        public decimal PorcentagemVagasDisponiveis => 100 - PorcentagemOcupacao;

        [NotMapped]
        public decimal PorcentagemPublicoAlvo => OcupacaoTotal == 0 ? 0 : PublicoAlvo * 100 / OcupacaoTotal;

        [NotMapped]
        public decimal PorcentagemPublicoGeral => OcupacaoTotal == 0 ? 0 : PublicoGeral * 100 / OcupacaoTotal;

        [NotMapped]
        public int OcupacaoTotal => PublicoAlvo + PublicoGeral;

        [NotMapped]
        public int VagasDisponiveis => Capacidade - OcupacaoTotal;


    }
}
