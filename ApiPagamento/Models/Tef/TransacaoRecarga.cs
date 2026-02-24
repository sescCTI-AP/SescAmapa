namespace PagamentoApi.Models.Tef
{
    public class TransacaoRecarga
    {
        public TransacaoRecarga(string cliente, string cpf, int cduop, int sqmatric, int nudv, string valor)
        {
            this.cliente = cliente;
            this.cpf = cpf;
            this.cduop = cduop;
            this.sqmatric = sqmatric;
            this.nudv = nudv;
            this.valor = valor;
        }

        public string cliente { get; set; }
        public string cpf { get; set; }
        public int cduop { get; set; }
        public int sqmatric { get; set; }
        public int nudv { get; set; }
        public string valor { get; set; }

    }
}
