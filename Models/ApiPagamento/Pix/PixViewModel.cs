using SiteSesc.Models.ApiPagamentoV2;
using SiteSesc.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixViewModel
    {
        public string Atividade { get; set; }
        public string DataCriacao { get; set; }
        public string Valor { get; set; }
        public string Horario { get; set; }
        public string Referencia { get; set; }
        public string TextoImagem { get; set; }
        public string Codigo { get; set; }

        public PixViewModel() { }

        public PixViewModel(CobrancaAtualizada cobranca, string imagem, string codigo)
        {
            Atividade = cobranca.atividade;
            //Valor = cobranca.valorRecebido.ToString().Replace(".", ",");
            Valor = cobranca.valorRecebido.ToString("F2", new System.Globalization.CultureInfo("pt-BR"));
            Referencia = $"{cobranca.vencimento.Month}/{cobranca.vencimento.Year}";
            TextoImagem = imagem;
            Codigo = codigo;
        }

        public PixViewModel(decimal valor, PixCriado pix, string descricao, string imagem)
        {
            Valor = valor.ToString().Replace(".", ",");
            DataCriacao = pix.calendario.criacao.ToShortDateString();
            Horario = pix.calendario.criacao.AddHours(1).ToShortTimeString();
            Atividade = descricao;
            TextoImagem = imagem;
            Codigo = pix.textoImagemQRcode;
        }

        public PixViewModel(decimal valor, string descricao, string dataCriacao, string imagem, string codigo)
        {
            Valor = valor.ToString().Replace(".", ",");
            Atividade = descricao;
            DataCriacao = dataCriacao;
            TextoImagem = imagem;
            Codigo = codigo;
        }

    }
}
