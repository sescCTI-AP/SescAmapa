using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SiteSesc.Models.Cardapio
{
    public class cdp_ItemCardapio
    {
        public int Id { get; set; }

        [Display(Name = "NOME DO ITEM")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        [ForeignKey("GrupoItemCardapio")]
        public int IdGrupoItemCardapio { get; set; }

        [Display(Name = "GRUPO NUTRICIONAL")]
        [ForeignKey("IdGrupoItemCardapio")]
        public virtual cdp_GrupoItemCardapio GrupoItemCardapio { get; set; }

        public virtual ICollection<cdp_ComposicaoCardapio> ComposicaoCardapio { get; set; }

    }
}