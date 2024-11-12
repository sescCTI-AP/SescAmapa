using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Telefone
    {
        public int Id { get; set; }

        [Display(Name = "NUMERO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Numero { get; set; }

        [Display(Name = "OBSERVACAO")]
        public string Observacao { get; set; }


        public virtual ICollection<TelefoneEmpresa> TelefoneEmpresa { get; set; }

    }
}
