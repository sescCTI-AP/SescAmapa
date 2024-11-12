using System;

namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixCalendario
    {
        public DateTime criacao { get; set; }
        public int expiracao { get; private set; }

        public PixCalendario(int expiracao)
        {
            this.expiracao = expiracao;
        }
    }
}
