using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.ProcessoSeletivo.ViewAction;
using SiteSesc.Models.ProcessoSeletivo;

namespace SiteSesc.Models.Edital
{
    public class edt_edital
    {
        public edt_edital()
        {
            edt_areasEdital = new HashSet<edt_areasEdital>();
            edt_cidadesEdital = new HashSet<edt_cidadesEdital>();
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

        [Display(Name = "DATA DO LANÇAMENTO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        
        public DateTime DataReuniao { get; set; }

        [Display(Name = "ABERTURA PARA ENVIO DE CURRÍCULO")]
        public DateTime? DataAberturaCurriculo { get; set; }

        [Display(Name = "FIM PARA ENVIO DE CURRÍCULO")]
        public DateTime? DataFimCurriculo { get; set; }

        [ForeignKey("edt_tipoEdital")]
        public int IdTipoEdital { get; set; }

        [ForeignKey("edt_statusEdital")]
        public int IdStatusEdital { get; set; }

        [ForeignKey("edt_modalidade")]
        public int IdModalidade { get; set; }

        public bool IsAtivo { get; set; } = true;

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("Area")]
        public int? IdArea { get; set; }

        [Display(Name = "CIDADE")]
        public string? Cidade { get; set; }


        [Display(Name = "TIPO DO EDITAL")]
        [ForeignKey("IdTipoEdital")]
        public virtual edt_tipoEdital edt_tipoEdital { get; set; }

        [Display(Name = "STATUS")]
        [ForeignKey("IdStatusEdital")]
        public virtual edt_statusEdital edt_statusEdital { get; set; }

        [Display(Name = "MODALIDADE")]
        [ForeignKey("IdModalidade")]
        public virtual edt_modalidade edt_modalidade { get; set; }

        [Display(Name = "USUÁRIO")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<edt_arquivosEdital> edt_arquivosEdital { get; set; }

        public virtual ICollection<edt_cargoEdital> edt_cargoEdital { get; set; }
        public ICollection<edt_areasEdital> edt_areasEdital { get; set; }
        public ICollection<edt_cidadesEdital> edt_cidadesEdital { get; set; }

        //Not Mapped Attributes
        [NotMapped]
        [Display(Name = "DOCUMENTO")]
        public IFormFile Documento { get; set; }

        [NotMapped]
        [Display(Name = "NOME DO DOCUMENTO")]
        public string NomeDocumento { get; set; }


        [Display(Name = "ÁREA DE COORDENAÇÃO")]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

    }
}
