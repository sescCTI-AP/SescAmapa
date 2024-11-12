using SiteSesc.Models.Admin;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ViewModel
{
    [NotMapped]
    public class AtualizacaoEmailViewModel
{
    public string Cpf { get; set; }
    public string NovoEmail { get; set; }
}
}

