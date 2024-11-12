using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SiteSesc.Models.Cardapio
{
    public class cdp_Cardapio
    {
        public int Id { get; set; }

        public DateTime? DataCadastro { get; set; }

        [Display(Name = "DATA DE INÍCIO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime? DataInicio { get; set; }

        [Display(Name = "DATA FINAL")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "ESTÁ ATIVO?")]
        public bool IsAtivo { get; set; }

        [Display(Name = "OBSERVAÇÃO")]
        public string Observacao { get; set; }



        [ForeignKey("UnidadeOperacional")]
        public int IdUnidadeOperacional { get; set; }

        [Display(Name = "UNIDADE")]
        [ForeignKey("IdUnidadeOperacional")]
        public virtual UnidadeOperacional UnidadeOperacional { get; set; }


        public virtual ICollection<cdp_ComposicaoCardapio> ComposicaoCardapio { get; set; }

    }
}