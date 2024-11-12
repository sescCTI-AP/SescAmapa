using System;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Endereco
    {
        public int Id { get; set; }

        [Display(Name = "LOGRADOURO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Logradouro { get; set; }

        [Display(Name = "NÚMERO")]
        public int? Numero { get; set; }

        [Display(Name = "BAIRRO")]
        public string Bairro { get; set; }

        [Display(Name = "COMPLEMENTO")]
        public string Complemento { get; set; }

        [Display(Name = "CIDADE")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Cidade { get; set; }

        [Display(Name = "ESTADO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Estado { get; set; }

        [Display(Name = "CEP")]
        public string Cep { get; set; }

    }
}