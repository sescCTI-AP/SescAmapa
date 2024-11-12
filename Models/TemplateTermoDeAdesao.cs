using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class TemplateTermoDeAdesao
    {
        public int Id { get; set; }

        [Display(Name = "PROGRAMA")]
        public string Programa { get; set; }

        [Display(Name = "PROGRAMA")]
        public int CdMapa { get; set; }

        [Display(Name = "ATIVO?")]
        public bool IsAtivo { get; set; }

        [Display(Name = "ANO")]
        public int Ano { get; set; }

        [Display(Name = "CONTEÚDO")]
        public string Conteudo { get; set; }
    }
}
