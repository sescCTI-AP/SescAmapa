using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Licitacao
{
    public class lct_statusLicitacao
    {
        public int Id { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }

        //0 = Aberto, 1 = Andamento, 2 = Finalizado
        public int Etapa { get; set; }
    }
}
