using System;

namespace PagamentoApi.Models.Partial
{
    public class CaixaPartial
    {
        private string nufechamento;
        public string NUFECHAMEN
        {
            get
            {
                return nufechamento;
            }
            set
            {
                nufechamento = value;
                Ano = value.Substring(0, 4);
                Numero = Convert.ToInt32(value.Substring(4, 4));
            }
        }
        public int SQCAIXA { get; set; }
        public int Numero { get; set; }
        public string Ano { get; set; }
    }

}