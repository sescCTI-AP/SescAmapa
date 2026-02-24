using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagamentoApi.Models.Site
{
    public class RelatorioIndicadores
    {
        public int ClientesPleno { get; set; }
        public int ClientesAtividade { get; set; }
        public int ClientesVencidos { get; set; }
        public int NovosCliente { get; set; }
        public int ClientesRenovados { get; set; }
        public int ClienteVencimentoNoMes { get; set; }
        public int InscritosMes { get; set; }
        public int EvasoesMes { get; set; }
        public VagasProdutividade VagasProdutividade { get; set; }
        public List<CLIENTELA> ClientesSemCpf { get; set; }
        public List<AtendenteRanking> AtendentesRaking { get; set; }
        public List<InscricoesEvasoesViewModel> InscricoesEvasoes { get; set; }
        public List<PlataformaInscricao> PlataformaInscricao { get; set; }
        public List<InscritosPorFaixaEtaria> InscritosPorFaixaEtaria { get; set; }
        public int ClientesMatriculados { get; set; }
        public int ClientesInadimplentes { get; set; }
        public List<HorarioAtendimento> HorariosAtendimento { get; set; }
    }

    public class HorarioAtendimento
    {
        public int Hora { get; set; }
        public int Count { get; set; }
    }
}
