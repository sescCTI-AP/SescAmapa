using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_documentoProcessoSeletivo
    {
        public int Id { get; set; }

        [ForeignKey("psl_cargoProcessoSeletivo")]
        public int IdCargoProcessoSeletivo { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }
        public string Cpf { get; set; }
        public string NomeCandidato { get; set; }
        public string Email { get; set; }

        [ForeignKey("psl_tipoDocumentoProcessoSeletivo")]
        public int IdTipoDocumentoProcessoSeletivo { get; set; }
        public DateTime DataCadastro { get; set; }


        [ForeignKey("IdArquivo")]
        public Arquivo Arquivo { get; set; }

        [ForeignKey("IdCargoProcessoSeletivo")]
        public psl_cargoProcessoSeletivo psl_cargoProcessoSeletivo { get; set; }

        [ForeignKey("IdTipoDocumentoProcessoSeletivo")]
        public psl_tipoDocumentoProcessoSeletivo psl_tipoDocumentoProcessoSeletivo { get; set; }
    }
}
