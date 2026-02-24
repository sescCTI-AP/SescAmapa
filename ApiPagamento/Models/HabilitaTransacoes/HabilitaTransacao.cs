using System;

namespace PagamentoApi.Models.HabilitaTransacoes
{
    public static class  HabilitaTransacao
    {
        public static bool IsHorarioDeGeracao()
        {
            return (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 30) ? false : true;
        }
        public static string messagem { get; set; } = "Não é permitido realizar recarga/pagamento entre às 23h31min e 23h59min.";
    }
}
