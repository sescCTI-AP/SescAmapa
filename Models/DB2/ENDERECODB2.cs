using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.DB2
{
    [NotMapped]
    public class ENDERECODB2
    {
        public int Id { get; set; }
        public string idclasse { get; set; }
        public string cdelement { get; set; }
        public int sqenderec { get; set; }
        public string dslogradou { get; set; }
        public string siestado { get; set; }
        public string dscomplem { get; set; }
        public int cdmunicip { get; set; }
        public int? nuimovel { get; set; }
        public string dsbairro { get; set; }
        public string nucep { get; set; }
        public int stprincip { get; set; }
        public DateTime? dtinicio { get; set; }
        public int smfieldatu { get; set; }
        public int? cduop { get; set; }
        public int? sqmatric { get; set; }
        public int? idendereco { get; set; }
        public string dsmunicip { get; set; }

    }
}
