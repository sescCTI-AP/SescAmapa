using SiteSesc.Models.Admin;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ViewModel
{
    [NotMapped]
    public class SetAutorizacaoViewModel
    {
        public string cpf { get; set; }
        public List<ModuloAcoesSistemaUsuario> listaAutorizacao { get; set; }
    }
}
