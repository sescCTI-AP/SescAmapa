using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ViewModel
{
    public class UsuarioResetSenha
    {
        public Guid Guid { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Compare("Senha", ErrorMessage = "As senhas são diferentes")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }
    }
}
