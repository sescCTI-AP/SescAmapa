using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SiteSesc.Models.Cardapio
{
    public class cdp_GrupoItemCardapio
    {
        public int Id { get; set; }


        [Display(Name = "NOME DO GRUPO/CATEGORIA")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        public virtual ICollection<cdp_ItemCardapio> cdp_ItemCardapio { get; set; }
    }
}