using System;

namespace PagamentoApi.Models
{
    public class ItemVenda
    {
        public string Produto { get; set; }
        public string Grupo { get; set; }
        public string Subgrupo { get; set; }
        public string UnidadeVenda { get; set; }
        public int CdUop { get; set; }
        public string LocalVenda { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal VlDescItem { get; set; }
        public string StatusVenda { get; set; }
        public int StVenda { get; set; }
        public string Dscancelam { get; set; }
        public DateTime DataVenda { get; set; }
        public string CategoriaCliente { get; set; }
        public bool EComerciario { get; set; }

    }
}
