using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.SiteViewModels
{
    public class InscritosViewModel
    {
        public string nmcliente { get; set; }
        public DateTime dtnascimen { get; set; }
         public DateTime dtinscri { get; set; }
         public DateTime dtstatus { get; set; }
        public int cduop { get; set; }
        public int sqmatric { get; set; }
        public string dscategori { get; set; }
        public int stinscri { get; set; }
        public int cdfonteinf { get; set; }
        public List<Contato> contatos { get; set; }

        [NotMapped]
        [Display(Name = "HABILITAÇÃO")]
        public string matricula =>  $"{cduop.ToString().PadLeft(4, '0')}-{sqmatric.ToString().PadLeft(6, '0')}";


    }

    public class Contato
    {
        public int sqmatric { get; set; }
        public int cduop { get; set; }
        public int sqcontato { get; set; }
        public int tpcontato { get; set; } 
        public int stprincipal { get; set; }
        public string nuddd { get; set; }
        public string dscontato { get; set; }
        public string nmpessoa { get; set; }
        public string strecebeinfo { get; set; }
    }
}
