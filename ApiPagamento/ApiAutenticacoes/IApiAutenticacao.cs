namespace PagamentoApi.ApiAutenticacoes
{
    public interface IApiAutenticacao
    {
        bool IsValidoChaveApi(string chaveApi);
    }
}
