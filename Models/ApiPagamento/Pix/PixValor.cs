using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixValor
    {
        public decimal original { get; private set; }

        public PixValor(decimal original)
        {
            this.original = original;
        }
    }
}
