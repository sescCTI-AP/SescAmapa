using SiteSesc.Models.Edital;

namespace SiteSesc.Models.Edital.ViewModel
{
    public class CargoViewModel
    {
        public List<edt_edital> Aguardando { get; set; }
        public List<edt_edital> Abertas { get; set; }
        public List<edt_edital> EmAndamento { get; set; }
        public List<edt_edital> Finalizadas { get; set; }        
        
        public edt_cargoEdital Cargo { get; set; }
        public int VagasDisponiveis { get; set; }
        public int CurriculosRecebidos { get; set; }
    }
}
