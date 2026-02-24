using System;

namespace PagamentoApi.Models.Pix
{
    public class SescTO_Pix_Transacoes
    {
        public int ID { get; set; }
        public string ENDTOENDID { get; set; }
        public string TXID { get; set; }
        public decimal VALOR { get; set; }
        public DateTime HORARIO { get; set; }
        public string CPF { get; set; }
        public string NOME { get; set; }
        public string INFORMACOES { get; set; }
        public SescTO_Pix_Transacoes()
        {

        }
        public SescTO_Pix_Transacoes(Pix pix)
        {
            ENDTOENDID = pix.EndToEndId;
            TXID = pix.TxId;
            VALOR = pix.Valor;
            HORARIO = pix.Horario;
            if (pix.Pagador.Cpf != "")
                CPF = pix.Pagador.Cpf;
            else
                CPF = pix.Pagador.Cnpj;
            NOME = pix.Pagador.Nome;
            INFORMACOES = pix.InfoPagador;
        }
    }
}