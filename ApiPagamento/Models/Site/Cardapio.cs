using System;
using System.Collections.Generic;

namespace PagamentoApi.Models.Site
{
    public class Cardapio
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public int DiaDaSemana { get; set; }
        public string Nome { get; set; }
        public string NomeUnidade { get; set; }
        public int IdUnidadeOperacional { get; set; }
        public int IdGrupoItemCardapio { get; set; }
    }

    public class CardapioUnidade
    {
        public string NomeUnidade { get; set; }
        public List<ItemCardapio> Itens { get; set; }
    }

    public class ItemCardapio
    {
        public string nome { get; set; }
        public int IdGrupoItemCardapio { get; set; }
    }

}