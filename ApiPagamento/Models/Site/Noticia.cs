using System;

namespace PagamentoApi.Models.Site
{
    public class Noticia
    {
        public string TituloLongo { get; set; }
        public string TituloCurto { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
        public string CorpoNoticia { get; set; }
        public string CaminhoVirtual { get; set; }
    }
}
