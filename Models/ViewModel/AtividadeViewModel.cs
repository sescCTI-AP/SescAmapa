using Microsoft.AspNetCore.Components.Web;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.Atividade;
using SiteSesc.Services;

namespace SiteSesc.Models.ViewModel
{
    public class AtividadeViewModel
    {
        public int Id { get; set; }
        public string NomeExibicao { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataInicioInscricao { get; set; }
        public DateTime? DataFimInscricao { get; set; }
        public DateTime? DataInicioAtividade { get; set; }
        public DateTime? DataFimAtividade { get; set; }
        public int NumeroVagas { get; set; }
        public int NumeroVagasOcupadas { get; set; }
        public string Uop { get; set; }
        public string Imagem { get; set; }
        public string Area { get; set; }
        public string SubArea { get; set; }
        public string Hrinicio { get; set; }
        public string Hrfim { get; set; }
        public List<AtividadeValor> Valores { get; set; }
        public bool DescontoPontualidade { get; set; }
        public string Cdelement { get; set; }

        public static AtividadeViewModel ToAtividade(AtividadeApi api, AtividadeOnLine site, List<Horario> horario, List<AtividadeValor> valores)
        {            
            return new AtividadeViewModel{
                Id = site.Id,
                NomeExibicao = site.NomeExibicao,
                Descricao = site.Descricao,
                DataInicioInscricao = api.dtlinscri,
                DataFimInscricao = api.dtuinscri,
                DataInicioAtividade = api.dtiniocorr,
                DataFimAtividade = api.dtfimocorr,
                NumeroVagas = api.nuvagas,
                NumeroVagasOcupadas = api.nuvagasocp,
                Uop = site.UnidadeOperacional.Nome,
                Imagem = site.Arquivo.CaminhoVirtualFormatado(),
                Area = site.SubArea.Area.Nome,
                SubArea = site.SubArea.Nome,
                Hrinicio = horario.Count() > 0 ? horario.FirstOrDefault().hrinicio : "",
                Hrfim = horario.Count() > 0 ? horario.FirstOrDefault().hrfim : "",
                Valores = valores,
                DescontoPontualidade = site.DescontoPontualidade,
                Cdelement = api.cdelement
            };
        }
    }
}
