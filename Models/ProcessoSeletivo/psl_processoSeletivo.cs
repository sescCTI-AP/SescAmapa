using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_processoSeletivo
    {
        public psl_processoSeletivo()
        {
            psl_areasProcessoSeletivo = new HashSet<psl_areasProcessoSeletivo>();
            psl_cidadesProcessoSeletivo = new HashSet<psl_cidadesProcessoSeletivo>();
            this.Guid = Guid.NewGuid();
        }
        public int Id { get; set; }

        public Guid Guid { get; set; }

        [Display(Name = "NÚMERO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string NumeroProcessoSeletivo { get; set; }

        [Display(Name = "CARGO")]
        public string Curso { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "CARGO(s)")]
        public string Cargo { get; set; }

        [Display(Name = "DATA DA PUBLICAÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataPublicacao { get; set; }

        [Display(Name = "ABERTURA PARA ENVIO DE CURRÍCULO")]
        public DateTime DataAberturaCurriculo { get; set; }

        [Display(Name = "FIM PARA ENVIO DE CURRÍCULO")]
        public DateTime DataFimCurriculo { get; set; }

        [ForeignKey("psl_tipoProcessoSeletivo")]
        public int IdTipoProcessoSeletivo { get; set; }

        [ForeignKey("psl_cidades")]
        [Display(Name = "CIDADE")]
        public string? Cidade { get; set; }

        [ForeignKey("psl_statusProcessoSeletivo")]
        public int IdStatusProcessoSeletivo { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("Area")]
        public int? IdArea { get; set; }

        [Display(Name = "TIPO DO PROCESSO")]
        [ForeignKey("IdTipoProcessoSeletivo")]
        public virtual psl_tipoProcessoSeletivo psl_tipoProcessoSeletivo { get; set; }

        [Display(Name = "STATUS")]
        [ForeignKey("IdStatusProcessoSeletivo")]
        public virtual psl_statusProcessoSeletivo psl_statusProcessoSeletivo { get; set; }

        [Display(Name = "ÁREA DE COORDENAÇÃO")]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

        [Display(Name = "USUÁRIO")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<psl_arquivosProcessoSeletivo> psl_arquivosProcessoSeletivo { get; set; }
        public virtual ICollection<psl_cargoProcessoSeletivo> psl_cargoProcessoSeletivo { get; set; }
        //public virtual ICollection<Curriculospsl_processoSeletivo> Curriculospsl_processoSeletivo { get; set; }
        public ICollection<psl_areasProcessoSeletivo> psl_areasProcessoSeletivo { get; set; }
        public ICollection<psl_cidadesProcessoSeletivo> psl_cidadesProcessoSeletivo { get; set; }

        //Not Mapped Attributes
        [NotMapped]
        [Display(Name = "DOCUMENTO")]
        public IFormFile Documento { get; set; }

        [NotMapped]
        [Display(Name = "NOME DO DOCUMENTO")]
        public string NomeDocumento { get; set; }

        [NotMapped]
        [Display(Name = "NOME DO CANDIDATO")]
        public string NomeCandidato { get; set; }

        [NotMapped]
        [Display(Name = "TELEFONE")]
        public string Telefone { get; set; }

        [NotMapped]
        [Display(Name = "EMAIL")]
        public string Email { get; set; }
    }
}
