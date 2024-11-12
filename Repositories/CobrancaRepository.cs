using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.ApiPagamento.Cielo;
using SiteSesc.Models.ApiPagamento.Pix;
using SiteSesc.Models.DB2;
using SiteSesc.Models.Relatorio;
using System.Text;

namespace SiteSesc.Repositories
{
    public class CobrancaRepository
    {
        private SiteSescContext _dbContext;
        private ClienteRepository _clienteRepository;
        public readonly HostConfiguration hostConfiguration;
        private readonly ApiClient _apiClient;
        private bool devMode;

        public CobrancaRepository([FromServices] SiteSescContext context, IConfiguration configuration, ApiClient apiClient, ClienteRepository clienteRepository)
        {
            devMode = configuration.GetSection("Development")["mode"] == "development";
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _dbContext = context;
            _apiClient = apiClient;
            _clienteRepository = clienteRepository;
        }

        public async Task<List<COBRANCA>> ObterCobrancaPorCpf(string cpf, int ano)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cobranca/{cpf}/{ano}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cobrancas = JsonConvert.DeserializeObject<List<COBRANCA>>(successContent);
                    return cobrancas;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<CobrancaAtualizada>> ObterProximasCobrancasPorCpf(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cobranca/proximas/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cobrancas = JsonConvert.DeserializeObject<List<CobrancaAtualizada>>(successContent);
                    return cobrancas;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Caixa> ObtemCaixaAberto()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/caixa");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<Caixa>(successContent);
                    return retorno;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Caixa> ObtemCaixaAbertoTotem()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/caixa/caixa-aberto-totem");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<Caixa>(successContent);
                    return retorno;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }



        public async Task<List<TransacaoRealizada>> ObterTransacoesCredito(int? caixa)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cielo/transacoes/{caixa}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var transacoes = JsonConvert.DeserializeObject<List<TransacaoRealizada>>(successContent);
                    return transacoes;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<TransacaoTotem>> ObterTransacoesTotem(int? caixa)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cielo/transacoes-totem/{caixa}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();                    
                    var transacoes = JsonConvert.DeserializeObject<List<TransacaoTotem>>(successContent);
                    return transacoes;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<string> GetPaymentId(string merchantOrder)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cielo/compras-pedido/{merchantOrder}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var transacoes = JsonConvert.DeserializeObject<RetornoMerchantOrder>(successContent);
                    return transacoes.payments.FirstOrDefault().paymentId;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PaymentValidacao> ValidaTransacoesCreditoCielo(string paymentId)
        {
            try
            {
                var paym = new PaymentString { PaymentId = paymentId };

                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(paym);
                var response = await _apiClient.Cliente.PostAsync($"/v1/cielo/dados-compra",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var transacoes = JsonConvert.DeserializeObject<PaymentValidacao>(successContent);
                    return transacoes;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<CobrancaAtualizada> ObterValorAtualizado(COBRANCA cobranca)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(cobranca);
                var response = await _apiClient.Cliente.PostAsync($"/v1/cobranca/valor-atualizado",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<CobrancaAtualizada>(successContent);
                    return retorno;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PixCriado> consultaPix(string txid)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/Pix/consultar/{txid}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cobrancas = JsonConvert.DeserializeObject<PixCriado>(successContent);
                    return cobrancas;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> VerificaPixPago(string txid)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/Pix/verificapix/{txid}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var status = JsonConvert.DeserializeObject<bool>(successContent);
                    return status;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<PixCriado> GetPix(PixCobranca pix)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(pix);

                var response = await _apiClient.Cliente.PostAsync($"/v1/Pix/gerar",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<PixCriado>(successContent);
                    return retorno;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PixCriado> GetPixRecarga(PixRecarga pix)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(pix);

                var response = await _apiClient.Cliente.PostAsync($"/v1/Pix/recarga",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<PixCriado>(successContent);
                    return retorno;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> Recarga(CartaoCredito recarga)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(recarga);
                var response = await _apiClient.Cliente.PostAsync($"/v1/cielo/recarga",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //var retorno = JsonConvert.DeserializeObject<string>(successContent);
                    return successContent;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> PagamentoCartao(Transacao transacao)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(transacao);
                var response = await _apiClient.Cliente.PostAsync($"/v1/cielo/pagar-cobranca",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();

                    return successContent;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> VerificaInadimplencia(List<ClientelaViewModel> listaClientes, string cpf)
        {
            if (!string.IsNullOrEmpty(cpf))
            {
                listaClientes = await _clienteRepository.ObterClienteDependentesPorCpf(cpf);
            }
            if (listaClientes != null)
            {
                foreach (var item in listaClientes)
                {
                    var anoIndimplencia = DateTime.Now.Year - 5;

                    //VERIFICA INADIMPLENCIA EM 5 ANOS
                    for (var i = anoIndimplencia; i <= DateTime.Now.Year; i++)
                    {
                        var cobrancas = await ObterCobrancaPorCpf(item.NUCPF, i);
                        if (cobrancas != null && cobrancas.Any())
                        {
                            if (cobrancas.Any(c => c.STRECEBIDO == 0 && c.DTVENCTO.Date < DateTime.Now.Date))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            return false;
        }
    }
}