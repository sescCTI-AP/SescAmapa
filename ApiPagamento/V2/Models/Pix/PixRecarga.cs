
namespace PagamentoApi.V2.Models.Pix
{
    public class PixRecarga
    {
        public int Cduop { get; set; }
        public int Sqmatric { get; set; }
        public int Cartao { get; set; }
        public int SqMoviment { get; set; }
        public PixCriar Pix { get; set; }
        public int Tipo { get; set; }
    }
}