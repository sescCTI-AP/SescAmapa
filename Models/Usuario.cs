using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Services;
using SiteSesc.Models.ProcessoSeletivo;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SiteSesc.Models.Admin;
using SiteSesc.Models.Avaliacao;

namespace SiteSesc.Models
{
    public class Usuario
    {

        public Usuario()
        {
            this.Guid = Guid.NewGuid();
            this.DataCadastro = DateTime.Now;
            this.IdPerfilUsuario = 2;
            this.Token = null;
            this.RefreshToken = null;
        }

        public int Id { get; set; }

        [Display(Name = "GUID")]
        public Guid Guid { get; set; }

        [Display(Name = "NOME")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(11)]
        public string Cpf { get; set; }

        [Display(Name = "DATA DE CADASTRO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataCadastro { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public bool IsEmpresa { get; set; }

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

        public bool IsAtivo { get; set; }

        public string Status => IsAtivo ? "Ativo" : "Desativado";
        public string ClassStatus => IsAtivo ? "ativo" : "desativado";

        [ForeignKey("PerfilUsuario")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public int IdPerfilUsuario { get; set; }

        [Display(Name = "PERFIL")]
        [ForeignKey("IdPerfilUsuario")]
        [BindNever]
        public virtual PerfilUsuario PerfilUsuario { get; set; }


        [BindNever]
        public virtual ICollection<adm_usuarioModuloSistema> adm_usuarioModuloSistema { get; set; }

        [BindNever]
        public virtual ICollection<SolicitacaoCadastroCliente> SolicitacaoCadastroCliente { get; set; }



        [BindNever]
        public virtual ICollection<Atividade.AtividadeOnLine> AtividadeOnLine { get; set; }

        [BindNever]
        public virtual ICollection<MensagemRapida> MensagemRapida { get; set; }

        [BindNever]
        public virtual ICollection<HstSolicitacao> HstSolicitacao { get; set; }

        [BindNever]
        public virtual ICollection<SubArea> SubArea { get; set; }


        [BindNever]
        public virtual ICollection<AvaliacaoAtividadeCliente> AvaliacaoAtividadeCliente { get; set; }

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
            Cpf = cpf.Replace(".","").Replace("-","");
        }




    }
}
