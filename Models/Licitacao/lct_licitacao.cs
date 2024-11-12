using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.ProcessoSeletivo;

namespace SiteSesc.Models.Licitacao
{
    public class lct_licitacao
    {
        public lct_licitacao()
        {
            this.Guid = Guid.NewGuid();
        }
        public int Id { get; set; }

        public Guid Guid { get; set; }

        [Display(Name = "NÚMERO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string NumeroEdital { get; set; }

        [Display(Name = "OBJETO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Objeto { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "DATA DA REUNIÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataReuniao { get; set; }

        [ForeignKey("lct_tipoLicitacao")]
        public int IdTipoLicitacao { get; set; }

        [ForeignKey("lct_statusLicitacao")]
        public int IdStatusLicitacao { get; set; }

        [ForeignKey("lct_modalidade")]
        public int IdModalidade { get; set; }

        public bool IsAtivo { get; set; } = true;

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Display(Name = "TIPO DA LICITAÇÃO")]
        [ForeignKey("IdTipoLicitacao")]
        public virtual lct_tipoLicitacao lct_tipoLicitacao { get; set; }

        [Display(Name = "STATUS")]
        [ForeignKey("IdStatusLicitacao")]
        public virtual lct_statusLicitacao lct_statusLicitacao { get; set; }

        [Display(Name = "MODALIDADE")]
        [ForeignKey("IdModalidade")]
        public virtual lct_modalidade lct_modalidade { get; set; }

        [Display(Name = "USUÁRIO")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<lct_arquivosLicitacao> lct_arquivosLicitacao { get; set; }

        //Not Mapped Attributes
        [NotMapped]
        [Display(Name = "DOCUMENTO")]
        public IFormFile Documento { get; set; }

        [NotMapped]
        [Display(Name = "NOME DO DOCUMENTO")]
        public string NomeDocumento { get; set; }


    }
}
