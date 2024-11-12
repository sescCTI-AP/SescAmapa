namespace SiteSesc.Models.ViewModel
{
    public class AtividadeOnLineDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ArquivoDTO Arquivo { get; set; }
        public UsuarioDTO Usuario { get; set; }
        public UnidadeOperacionalDTO UnidadeOperacional { get; set; }
        public SubAreaDTO SubArea { get; set; }
    }

    public class ArquivoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class UnidadeOperacionalDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class SubAreaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public AreaDTO Area { get; set; }
    }

    public class AreaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

}
