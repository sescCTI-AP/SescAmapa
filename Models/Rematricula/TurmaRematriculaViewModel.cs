using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Rematricula
{
    [NotMapped]
    public class TurmaRematriculaViewModel
    {
        public int Id { get; set; }
        public string Unidade { get; set; }
        public string TurmaAnterior { get; set; }
        public string cdelementTurmaPermitida { get; set; }
        public string cdelementAtvAssociada { get; set; }
        public string TurmaPermitida { get; set; }
        public string Material { get; set; }
    }
}
