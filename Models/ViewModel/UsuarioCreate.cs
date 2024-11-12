using SiteSesc.Models.Enums;
using SiteSesc.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ViewModel
{
    public class UsuarioCreate
    {
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(11)]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Username { get; set; }

        [Display(Name = "SENHA")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [NotMapped]
        [Display(Name = "CONFIRMAR SENHA")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Compare("Senha", ErrorMessage = "As senhas são diferentes")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }

        [Display(Name = "EMAIL")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [NotMapped]
        [Display(Name = "EMAIL")]
        [Compare("Email", ErrorMessage = "Os Emails são diferentes")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [DataType(DataType.EmailAddress)]
        public string ConfirmaEmail { get; set; }

        public void SetSenha(string senha)
        {
            //var validaSenha = Seguranca.GetForcaDaSenha(senha);
            //var forcaDaSenha = Convert.ToInt32(validaSenha);
            //if (forcaDaSenha < 2)
            //    throw new ArgumentException("Senha fraca");
            Senha = Seguranca.Sha256(senha);
        }

        public void SetCpf(string cpf)
        {
            if (!Util.ValidaCpf(cpf))
                throw new ArgumentException("O CPF não é válido");
            Cpf = cpf.Replace(".", "").Replace("-", "");
        }


        public static Usuario ToUsuario(UsuarioCreate usuario)
        {
            return new Usuario
            {
                Nome = usuario.Nome,
                Username = usuario.Username,
                Senha = usuario.Senha,
                Cpf = usuario.Cpf,
                Email = usuario.Email,
                Guid = Guid.NewGuid(),
                DataCadastro = DateTime.Now,
                IdPerfilUsuario = (int)PerfilUsuarioEnum.Cliente
            };
        }
    }
}
