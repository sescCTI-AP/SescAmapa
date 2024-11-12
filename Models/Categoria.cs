namespace SiteSesc.Models
{
    public class Categoria
    {
        public Categoria(string nome, int cdCategoria)
        {
            Nome = nome;
            CdCategoria = cdCategoria;
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public int CdCategoria { get; set; }
    }
}
