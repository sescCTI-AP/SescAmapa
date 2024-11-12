using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Display(Name = "RAZÃO SOCIAL")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string RazaoSocial { get; set; }

        [Display(Name = "NOME FANTASIA")]
        public string NomeFantasia { get; set; }

        [Display(Name = "RAMO")]
        public string Ramo { get; set; }

        [Display(Name = "EMAIL")]
        public string Email { get; set; }

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Cnpj { get; set; }

        [ForeignKey("Endereco")]
        public int? IdEndereco { get; set; }

        [Display(Name = "ENDEREÇO")]
        [ForeignKey("IdEndereco")]
        public virtual Endereco Endereco { get; set; }


        public virtual ICollection<TelefoneEmpresa> TelefoneEmpresa { get; set; }

    }
}
