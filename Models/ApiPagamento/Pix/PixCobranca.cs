using SiteSesc.Models.DB2;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixCobranca
    {
        public int Cduop { get; private set; }
        public int Sqmatric { get; private set; }
        public string CdElement { get; private set; }
        public int SqCobranca { get; private set; }
        public string IdClasse { get; private set; }
        public decimal Valor { get; private set; }
        public decimal Juros { get; private set; }
        public decimal Multa { get; private set; }
        public decimal Desconto { get; private set; }
        public PixCriar Pix { get; private set; }
        public int Tipo { get; private set; }

        public PixCobranca(int cduop, int sqmatric, string cdElement, int sqCobranca, decimal valor, decimal juros, decimal multa, decimal desconto, int tipo, PixCriar pix)
        {
            Cduop = cduop;
            Sqmatric = sqmatric;
            CdElement = cdElement;
            SqCobranca = sqCobranca;
            Valor = valor;
            Juros = juros;
            Multa = multa;
            Desconto = desconto;
            Pix = pix;
            IdClasse = "OCRID";
            Tipo = tipo;
        }


        public PixCobranca(CobrancaAtualizada cobranca, ClienteCentral cliente, int tipo, string cpfPagador, string nomePagador)
        {
            var pixDevedor = new PixDevedor(cpfPagador, nomePagador);
            var pixValor = new PixValor(cobranca.valorRecebido);
            var pixCriar = new PixCriar(pixDevedor, pixValor, cobranca.atividade);

            Cduop = cliente.Cduop;
            IdClasse = "OCRID";
            Sqmatric = cliente.Sqmatric;
            CdElement = cobranca.cdelement;
            SqCobranca = cobranca.sqcobranca;
            Valor = cobranca.valorOriginal;
            Juros = cobranca.jurosMora;
            Multa = cobranca.multa;
            Desconto = cobranca.descontoConcedido;
            Tipo = tipo;
            Pix = pixCriar;
        }


    }
}
