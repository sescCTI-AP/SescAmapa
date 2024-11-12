using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Edital.ViewModel
{
    public class AvaliacaoDocumentacaoViewModel
    {
        /*var idCargo = $('#idCargo').val();
        var cpf = $('#cpfAvaliacaoCandidato').val();
        var idCargo = $('#idCargo').val();
        var statusCurriculo = $('#statusCurriculo').val();
        var Parecer = $('#Parecer').val();*/
        public int? Id { get; set; }
  

        public int IdCargo { get; set; }
        public int idEdital { get; set; }

        public string Cpf{  get; set; }

        public int StatusCurriculo { get; set; }
        public string Justificativa { get; set; }
        

    }
}
