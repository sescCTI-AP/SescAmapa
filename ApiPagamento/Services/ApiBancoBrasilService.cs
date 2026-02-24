using System.Net.Http;
using System.Threading.Tasks;
using System;
using PagamentoApi.Models.Responses;
using PagamentoApi.Settings;
using Newtonsoft.Json;

namespace PagamentoApi.Services
{
    public class ApiBancoBrasilService
    {
        public async Task<ResponseApi<TransacaoPixResponse>> BuscarPorTxidAsync(string txid)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("client_id", ConfigurationApi.ApiPixClientId);
                    httpClient.DefaultRequestHeaders.Add("client_secret", ConfigurationApi.ApiPixClientSecret);

                    HttpResponseMessage response = await httpClient.GetAsync($"{ConfigurationApi.ApiPixUrl}/api/pix/v2/cob/{txid}");
                    var responseDataString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        return new ResponseApi<TransacaoPixResponse?>(null, "Falhar Interna no Servidor", false);

                    var responseData = JsonConvert.DeserializeObject<ResponseApi<TransacaoPixResponse>>(responseDataString);

                    return new ResponseApi<TransacaoPixResponse>(responseData.Content, "Operacao realizada com sucesso !!!");
                }
            }
            catch (Exception ex)
            {
                return new ResponseApi<TransacaoPixResponse?>(null, "Error Interno no Servidor", false);
            }
        }
    }
}
