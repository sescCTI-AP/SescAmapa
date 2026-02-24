using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagamentoApi.Models.Cielo;
using PagamentoApi.Settings;

namespace PagamentoApi.Clients
{

    public class CieloClient
    {
        private readonly HttpClient _client;
        private readonly CieloApiSettings _apiSettings;
        private readonly string _urlBaseApi;
        private readonly string _urlBaseApiQuery;
        public CieloClient(HttpClient client, CieloApiSettings apiSettings)
        {
            _client = client;
            _apiSettings = apiSettings;
            _client.DefaultRequestHeaders.Add("ContentType", "application/json");
            if (_apiSettings.Sandbox)
            {
                //desenvolvimento
                _client.DefaultRequestHeaders.Add("MerchantId", _apiSettings.MerchantIdSandbox);
                _client.DefaultRequestHeaders.Add("MerchantKey", _apiSettings.MerchantKeySandbox);
                _urlBaseApi = _apiSettings.BaseApiSandbox;
                _urlBaseApiQuery = _apiSettings.BaseApiQuerySandbox;
            }
            else
            {
                //produção
                _urlBaseApi = _apiSettings.BaseApi;
                _urlBaseApiQuery = _apiSettings.BaseApiQuery;
                _client.DefaultRequestHeaders.Add("MerchantId", _apiSettings.MerchantId);
                _client.DefaultRequestHeaders.Add("MerchantKey", _apiSettings.MerchantKey);
            }
        }

        /// <summary>
        ///  Retorna os seguintes dados do Cartão:
        /// Bandeira do cartão: Nome da Bandeira
        /// -Tipo de cartão: Crédito, Débito ou Múltiplo(Crédito e Débito)
        /// -Nacionalidade do cartão: Estrangeiro ou Nacional
        /// -Cartão Corporativo: Se o cartão é ou não é corporativo
        /// -Banco Emissor: Código e Nome
        /// </summary>
        /// <param name="numeroCartao"></param>
        /// <returns></returns>
        public async Task<CardData> GetDadosCartaoByNumero(string numeroCartao)
        {
            try
            {
                var response = await _client.GetAsync($"{_urlBaseApiQuery}1/cardBin/{numeroCartao.Substring(0, 6)}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    var dataCard = JsonConvert.DeserializeObject<CardData>(successContent);
                    return dataCard;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<CardData>>(errorContent).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Usa o recurso ZeroAuth para validar os dados do cartão. Esse processo permite que o lojista saiba se o cartão é valido ou não antes de enviar a transação para autorização.
        /// Só funciona para as bandeiras: Visa, MasterCard e Elo
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public async Task<bool> ValidaCartao(Card card)
        {
            try
            {
                var contentJson = JsonConvert.SerializeObject(card);
                var response = await _client.PostAsync($"{_urlBaseApi}1/zeroauth",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnCardValidate>(successContent).Valid;
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

        /// <summary>
        /// Prepara uma compra, enviado todos os dados do produto e verificando se o Cliente possui ou não saldo suficiente. 
        /// Pode ser feita captura automatica.
        /// </summary>
        /// <param name="codeTrasaction"></param>
        /// <param name="valor"></param>
        /// <param name="card"></param>
        /// <param name="customer"></param>
        /// <returns>
        /// Status 0	NotFinished	ALL	Aguardando atualização de status
        /// Status 1	Authorized ALL Pagamento apto a ser capturado ou definido como pago
        /// Status 2	PaymentConfirmed ALL Pagamento confirmado e finalizado
        /// Status 3	Denied CC + CD + TF Pagamento negado por Autorizador
        /// Status 10	Voided ALL Pagamento cancelado
        /// Status 11	Refunded CC + CD Pagamento cancelado após 23:59 do dia de autorização
        /// Status 12	Pending ALL Aguardando Status de instituição financeira
        /// Status 13	Aborted ALL Pagamento cancelado por falha no processamento ou por ação do AF
        /// Status 20	Scheduled CC  Recorrência agendada
        /// </returns>
        public async Task<ReturnAuthorization> Autorizacao(Transacao transacao)
        {
            try
            {
                //transacao.MerchantOrderId = codeTrasaction.ToString();
                transacao.Payment.Type = TypeCard.CreditCard.ToString(); //Tipo cartão de Credito
                transacao.Payment.Installments = transacao.Payment.Installments > 0 ? transacao.Payment.Installments : 1; // Numero de parcelas
                transacao.Payment.SoftDescriptor = "Online"; //Nome na fatura do cliente - maximo 13 caracteres

                var contentJson = JsonConvert.SerializeObject(transacao);
                //Console.WriteLine(contentJson);
                var response = await _client.PostAsync($"{_urlBaseApi}1/sales",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnAuthorization>(successContent);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<ReturnAuthorization>>(errorContent).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Obtem as compras de um perdido
        /// </summary>
        /// <param name="merchantOrderId"></param>
        /// <returns></returns>
        public async Task<ReturnPayments> GetCompraPorPedido(string merchantOrderId)
        {
            try
            {
                var response = await _client.GetAsync($"{_urlBaseApiQuery}1/sales/?merchantOrderId={merchantOrderId}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnPayments>(successContent);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<ReturnPayments>>(errorContent).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        public async Task<Transacao> GetCompraPorTid(string Tid)
        {
            try
            {
                var response = await _client.GetAsync($"{_urlBaseApiQuery}1/sales/acquirerTid/{Tid}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    return JsonConvert.DeserializeObject<Transacao>(successContent);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<Transacao>>(errorContent).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Faz o lançamento de uma compra no cartão do cliente
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public async Task<ReturnTransacao> Captura(Guid paymentId)
        {
            try
            {
                var response = await _client.PutAsync($"{_urlBaseApi}1/sales/{paymentId}/capture",
                    new StringContent("", Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnTransacao>(successContent);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<ReturnTransacao>>(errorContent).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Cancela um compra
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="valorCompra"></param>
        /// <returns></returns>
        public async Task<ReturnTransacao> Cancelar(Guid paymentId, decimal valorCompra)
        {
            try
            {
                var response = await _client.PutAsync($"{_urlBaseApi}1/sales/{paymentId}/void?amount={valorCompra}",
                    new StringContent("", Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnTransacao>(successContent);

                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<ReturnTransacao>>(errorContent).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Falha ao cancelar a comprar: \n{e.Message}");
            }
        }

        /// <summary>
        /// Obtem os dados de uma Compra
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public async Task<ReturnAuthorization> GetCompra(Guid paymentId)
        {
            try
            {
                var response = await _client.GetAsync($"{_urlBaseApiQuery}1/sales/{paymentId}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnAuthorization>(successContent);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<List<ReturnAuthorization>>(errorContent).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Gera um Guid a partir dos dados do cartão, a fim de facilitar transações futuras.
        /// Obs.: funcina apenas com os dados de um cartão real.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public async Task<ReturnTokenizacao> TokenizacaoCartao(Card card)
        {
            try
            {
                var cardRequest = new
                {
                    CustomerName = card.Holder,
                    card.CardNumber,
                    card.Holder,
                    card.ExpirationDate,
                    card.Brand
                };
                var contentJson = JsonConvert.SerializeObject(cardRequest);
                var response = await _client.PostAsync($"{_urlBaseApi}1/card", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    PrintJson(successContent);
                    return JsonConvert.DeserializeObject<ReturnTokenizacao>(successContent);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ReturnTokenizacao>(errorContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //public ReturnAuthorization AutorizacaoComAntiFraude(Guid codeTrasaction, decimal valor, Card card, Cliente customer)
        //{
        //    try
        //    {
        //        var transacao = new
        //        {
        //            MerchantOrderId = codeTrasaction.ToString(),
        //            Payment = new
        //            {
        //                Type = "CreditCard",
        //                Amount = valor,
        //                Installments = 1,
        //                SoftDescriptor = "teste",
        //                CreditCard = card,
        //                Customer = customer,
        //                ServiceTaxAmount = 0
        //            },
        //            FraudAnalysis = new
        //            {
        //                Provider = "cybersource",
        //                Sequence = "AnalyseFirst",
        //                SequenceCriteria = "Always",
        //                CaptureOnLowRisk = false,
        //                VoidOnHighRisk = false,
        //                TotalOrderAmount = valor,
        //                FingerPrintId = "074c1ee676ed4998ab66491013c565e2",
        //                Browser = new
        //                {
        //                    CookiesAccepted = false,
        //                    Email = customer.Email,
        //                    IpAddress = "200.190.150.350",
        //                }
        //            }
        //        };
        //        var contentJson = JsonConvert.SerializeObject(transacao);
        //        var response = _client.PostAsync($"{_urlBaseApi}1/sales", new StringContent(contentJson, Encoding.UTF8, "application/json")).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var successContent = response.Content.ReadAsStringAsync().Result;
        //            PrintJson(successContent);
        //            return JsonConvert.DeserializeObject<ReturnAuthorization>(successContent);
        //        }
        //        var errorContent = response.Content.ReadAsStringAsync().Result;
        //        Console.WriteLine(errorContent);
        //        return JsonConvert.DeserializeObject<List<ReturnAuthorization>>(errorContent).FirstOrDefault();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        return null;
        //    }
        //}

        private static void PrintJson(string jsonContent)
        {
            var parsed = JObject.Parse(jsonContent);
            foreach (var pair in parsed)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
        }
    }
}