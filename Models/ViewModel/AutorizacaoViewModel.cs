using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ViewModel
{
    [NotMapped]
    public class AutorizacaoViewModel
    {
        public string cpf { get; set; }
        public bool isAtivo { get; set; }
        public string email { get; set; }
        public string idPerfilUsuario { get; set; }
    }
}
