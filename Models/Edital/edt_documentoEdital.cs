using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Edital
{
    public class edt_documentoEdital
    {
        public int Id { get; set; }

        [ForeignKey("edt_cargoEdital")]
        public int IdCargoEdital { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }
        public string Cpf { get; set; }
        public string NomeCandidato { get; set; }
        public string Email { get; set; }

        [ForeignKey("edt_tipoDocumentoEdital")]
        public int IdTipoDocumentoEdital { get; set; }
        public DateTime DataCadastro { get; set; }


        [ForeignKey("IdArquivo")]
        public Arquivo Arquivo { get; set; }

        [ForeignKey("IdCargoEdital")]
        public edt_cargoEdital edt_cargoEdital { get; set; }

        [ForeignKey("IdTipoDocumentoEdital")]
        public edt_tipoDocumentoEdital edt_tipoDocumentoEdital { get; set; }

        public string CaminhoVirtualFormatado;
        /*
        [ForeignKey("edt_parecerEdital")]
        public int parecerEdital { get; set; }*/
    }
}
