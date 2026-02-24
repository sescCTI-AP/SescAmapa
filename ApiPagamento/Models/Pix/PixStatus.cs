namespace PagamentoApi.Models.Pix
{
    public enum PixStatus2
    {
        ATIVA,
        CONCLUIDA,
        EM_PROCESSAMENTO,
        NAO_REALIZADO,
        DEVOLVIDO,
        REMOVIDA_PELO_USUARIO_RECEBEDOR,
        REMOVIDA_PELO_PSP
    }
}


// ATIVA: a cobrança está disponível, porém ainda não ocorreu pagamento;
// CONCLUIDA: a cobrança encontra-se paga. Não se pode alterar e nem remover uma cobrança cujo status esteja ?CONCLUÍDA?;
// EM_PROCESSAMENTO: liquidação em processamento;
// NAO_REALIZADO: indica que a devolução não pode ser realizada em função de algum erro durante a liquidação, como por exemplo, saldo insuficiente.;
// DEVOLVIDO: cobrança com devolução realizada pelo Sistema de Pagamentos Instantâneos (SPI);
// REMOVIDA_PELO_USUARIO_RECEBEDOR: foi solicitada a remoção da cobrança; a critério do usuário;
// REMOVIDA_PELO_PSP: recebedor, por conta de algum critério, solicitou a remoção da cobrança.