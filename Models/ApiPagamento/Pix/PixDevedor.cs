
namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixDevedor
    {
        public string cpf { get; private set; }
        public string? cnpj { get; private set; }
        public string nome { get; private set; }

        public PixDevedor(string cpf, string nome, string? cnpj = null)
        {
            this.cpf = cpf;
            this.cnpj = cnpj;
            this.nome = nome;
        }
    }
}
