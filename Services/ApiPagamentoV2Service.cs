using Newtonsoft.Json;
using SiteSesc.Configuracoes;
using SiteSesc.Models.ApiPagamentoV2;
using SiteSesc.Models.Responses;
using System.Text;

namespace SiteSesc.Services
{
    public class ApiPagamentoV2Service(HttpClient _httpClient)
    {
        public async Task<ResponseApi<RecargaPixResponse?>> CobrancaPixCriarAsync(CobrancaPixRequest request)
        {
            try
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
                var conteudo = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("/v2/cobrancas/pix-criar", conteudo);
                var responseDataString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new ResponseApi<RecargaPixResponse?>(null, "Falhar Interna no Servidor", false);

                var responseData = JsonConvert.DeserializeObject<ResponseApi<RecargaPixResponse>>(responseDataString);
                return new ResponseApi<RecargaPixResponse?>(responseData?.Content, "Operacao realizada com sucesso !!!");
            }
            catch (Exception ex)
            {
                return new ResponseApi<RecargaPixResponse?>(null, "Error Interno no Servidor", false);
            }
        }
        public async Task<ResponseApi<string?>> CobrancaCartaoCieloAsync(CobrancaCartaoCieloRequest request)
        {
            try
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
                var conteudo = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("/v2/cobrancas/cartao-credito", conteudo);
                var responseDataString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return new ResponseApi<string?>(null, "Falhar Interna no Servidor", false);

                var responseData = JsonConvert.DeserializeObject<ResponseApi<string>>(responseDataString);
                return new ResponseApi<string?>(responseData?.Content, "Operacao realizada com sucesso !!!");
            }
            catch (Exception ex)
            {
                return new ResponseApi<string?>(null, "Error Interno no Servidor", false);
            }
        }
        
        public async Task<ResponseApi<RecargaPixResponse?>> RecargaPixCriarAsync(RecargaPixRequest request)
        {
            try
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
                var conteudo = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/v2/recarga/pix-criar", conteudo);

                var responseDataString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new ResponseApi<RecargaPixResponse?>(null, "Falhar Interna no Servidor", false);

                var responseData = JsonConvert.DeserializeObject<ResponseApi<RecargaPixResponse>>(responseDataString);

                return new ResponseApi<RecargaPixResponse?>(responseData?.Content, "Operacao realizada com sucesso !!!");

            }
            catch (Exception ex)
            {
                return new ResponseApi<RecargaPixResponse?>(null, "Error Interno no Servidor", false);
            }

        }
        
        public async Task<ResponseApi<string?>> RecargaCartaoCieloAsync(RecargaCartaoCieloRequest request)
        {
            try
            {
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(request);   
                var conteudo = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"/v2/recarga/cartao-credito", conteudo);

                if (!response.IsSuccessStatusCode)
                    return new ResponseApi<string?>(null, "Falhar Interna no Servidor", false);

                var responseDataString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<ResponseApi<string>>(responseDataString);

                return new ResponseApi<string?>(responseData?.Content, "Operacao realizada com sucesso !!!");
                
            }
            catch (Exception ex)
            {
                return new ResponseApi<string?>(null, "Error Interno no Servidor", false);
            }

        }
        
    }
}
