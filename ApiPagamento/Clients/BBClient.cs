using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagamentoApi.Models.BB;
using PagamentoApi.Models.Pix;
using PagamentoApi.Models.Boleto;
using PagamentoApi.Repositories;
using PagamentoApi.Settings;

namespace PagamentoApi.Clients
{
    public class BBClient
    {
        private readonly HttpClient _client;
        private readonly BBApiSettings _apiSettings;
        private readonly BoletoRepository _boletoRepository;
        private readonly PixRepository _pixRepository;
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly string _urlBaseApi;
        private readonly string _urlOauth;
        private readonly string _appKey;
        private readonly string _agencia;
        private readonly string _conta;
        private readonly string _convenio;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _chavePix;

        private Token token;

        public BBClient(HttpClient client, BBApiSettings apiSettings, BoletoRepository boletoRepository, PixRepository pixRepository, CobrancaRepository cobrancaRepository)
        {
            _client = client;
            _apiSettings = apiSettings;
            _boletoRepository = boletoRepository;
            _pixRepository = pixRepository;
            _cobrancaRepository = cobrancaRepository;

            _client.DefaultRequestHeaders.Add("ContentType", "application/json");
            if (_apiSettings.Sandbox)
            {
                //desenvolvimento
                _clientId = _apiSettings.ClientIdSandbox;
                _clientSecret = _apiSettings.ClientSecretSandbox;
                _urlBaseApi = _apiSettings.BaseApiSandbox;
                _urlOauth = _apiSettings.BaseOauthSandbox;
                _appKey = _apiSettings.AppKeySandbox;
                _agencia = _apiSettings.AgenciaSandbox;
                _conta = _apiSettings.ContaSandbox;
                _convenio = _apiSettings.ConvenioSandbox;
                _chavePix = _apiSettings.ChavePixSandbox;
            }
            else
            {
                //produção
                _clientId = _apiSettings.ClientId;
                _clientSecret = _apiSettings.ClientSecret;
                _urlBaseApi = _apiSettings.BaseApi;
                _urlOauth = _apiSettings.BaseOauth;
                _appKey = _apiSettings.AppKey;
                _agencia = _apiSettings.Agencia;
                _conta = _apiSettings.Conta;
                _convenio = _apiSettings.Convenio;
                _chavePix = _apiSettings.ChavePix;
            }

        }

        public async Task<Token> ObterToken()
        {
            try
            {
                var dados = new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "cobrancas.boletos-requisicao cobrancas.boletos-info cob.write cob.read pix.write pix.read"),
                };

                var formContent = new FormUrlEncodedContent(dados);
                string client_credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                _client.DefaultRequestHeaders.Add("Authorization", $"Basic {client_credentials}");
                _client.DefaultRequestHeaders.Add("ContentType", "x-www-form-urlencoded");
                var response = await _client.PostAsync($"{_urlOauth}token", formContent);
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //PrintJson(successContent);
                    token = JsonConvert.DeserializeObject<Token>(successContent);
                    return token;
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

        public async Task<BoletoResponse> ListarBoletos(char situacao = 'B', int codigoEstadoTituloCobranca = 0, DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }

                var dados = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("gw-dev-app-key", _appKey),
                        new KeyValuePair<string, string>("indicadorSituacao", situacao.ToString()),
                        new KeyValuePair<string, string>("agenciaBeneficiario", _agencia),
                        new KeyValuePair<string, string>("contaBeneficiario", _conta),
                    };
                if (codigoEstadoTituloCobranca != 0) //B -  05, 06, 07 ou 09
                {
                    dados.Add(new KeyValuePair<string, string>("codigoEstadoTituloCobranca", Convert.ToString(codigoEstadoTituloCobranca)));
                }
                if (situacao == 'B' && dataInicial != null && dataFinal != null)
                {
                    dados.Add(new KeyValuePair<string, string>("dataInicioMovimento", dataInicial.Value.ToString("dd.MM.yyyy")));
                    dados.Add(new KeyValuePair<string, string>("dataFimMovimento", dataFinal.Value.ToString("dd.MM.yyyy")));
                }
                if (situacao == 'A' && dataInicial != null && dataFinal != null)
                {
                    dados.Add(new KeyValuePair<string, string>("dataInicioRegistro", dataInicial.Value.ToString("dd.MM.yyyy")));
                    dados.Add(new KeyValuePair<string, string>("dataFimRegistro", dataFinal.Value.ToString("dd.MM.yyyy")));
                }
                //dataInicioRegistro
                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");
                var response = await _client.GetAsync($"{_urlBaseApi}cobrancas/v2/boletos?{query}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //PrintJson(successContent);
                    var dataCard = JsonConvert.DeserializeObject<BoletoResponse>(successContent);
                    return dataCard;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return JsonConvert.DeserializeObject<BoletoResponse>(errorContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> ObterDetalhesBoletos(string id)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }

                var dados = new[]
                {
                    new KeyValuePair<string, string>("gw-dev-app-key", _appKey),
                    new KeyValuePair<string, string>("numeroConvenio", _convenio),
                };
                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");
                var response = await _client.GetAsync($"{_urlBaseApi}cobrancas/v2/boletos/{id}?{query}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //PrintJson(successContent);
                    var detalhes = JsonConvert.DeserializeObject<DetalhamentoBoleto>(successContent);
                    return detalhes;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return errorContent;
                //return JsonConvert.DeserializeObject<DetalhamentoBoleto>(errorContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> BaixarBoleto(string id)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }

                var dados = new[]
                {
                    new KeyValuePair<string, string>("gw-dev-app-key", _appKey),
                    new KeyValuePair<string, string>("numeroConvenio", _convenio),
                };
                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");
                var bodyContent = new
                {
                    numeroConvenio = _convenio
                };
                var contentJson = JsonConvert.SerializeObject(bodyContent);
                var response = await _client.PostAsync($"{_urlBaseApi}cobrancas/v2/boletos/{id}/baixar?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    //PrintJson(successContent);
                    var detalhes = JsonConvert.DeserializeObject<RespostaBaixaBoleto>(successContent);
                    return detalhes;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return errorContent;
                //return JsonConvert.DeserializeObject<DetalhamentoBoleto>(errorContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> GerarBoleto(BoletoRequest boleto)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }

                var hasBoleto = await _boletoRepository.VerificaBoleto(boleto.CdElement, boleto.SqCobranca);
                if (hasBoleto != null)
                {
                    var boletoGerado = new RespostaRegistroBoletos
                    {
                        Numero = hasBoleto.NOSSO_NUMERO,
                        LinhaDigitavel = hasBoleto.LINHA_DIGITAVEL,
                        CodigoBarraNumerico = hasBoleto.CODIGO_BARRAS,
                        NumeroCarteira = 17,
                        NumeroVariacaoCarteira = 35,
                        CodigoCliente = 0
                    };
                    var qrcode = new QrCode
                    {
                        Emv = hasBoleto.QRCODE_EMV,
                        TxId = hasBoleto.QRCODE_TXID,
                        Url = hasBoleto.QRCODE_URL
                    };

                    var beneficiario = new Beneficiario
                    {
                        Agencia = Convert.ToDouble(_agencia),
                        ContaCorrente = Convert.ToDouble(_conta)
                    };
                    boletoGerado.QrCode = qrcode;
                    boletoGerado.Beneficiario = beneficiario;
                    return boletoGerado;
                }
                else
                {
                    boleto.IndicadorPix = "S";
                    boleto.NumeroConvenio = Convert.ToInt32(_convenio);
                    //boleto.numeroCarteira = Convert.ToInt32(_carteira);
                    boleto.CodigoModalidade = 1;
                    boleto.DataEmissao = DateTime.Now.ToString("dd.MM.yyyy");
                    //boleto.IndicadorAceiteTituloVencido = "S";
                    boleto.CodigoTipoTitulo = 4;
                    boleto.CodigoAceite = "A";

                    //Seu Numero - Utilizar cdelement????
                    //boleto.NumeroTituloBeneficiario = "TESTE123";

                    //Nosso numero "00031285570000447004"
                    var nossoNumero = await _boletoRepository.ObterNossoNumero(_convenio);
                    nossoNumero.NUMERO++;
                    
                    boleto.NumeroTituloCliente = "000" + _convenio + Convert.ToString(nossoNumero.NUMERO).PadLeft(10, '0');

                    boleto.CampoUtilizacaoBeneficiario = "";
                    boleto.MensagemBloquetoOcorrencia = "Boleto válido para pagamento somente até a data de vencimento.";

                    var dados = new[]
                    {
                        new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                    };
                    var formContent = new FormUrlEncodedContent(dados);
                    var query = formContent.ReadAsStringAsync().Result;
                    _client.DefaultRequestHeaders.Remove("Authorization");
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");
                    //var response = await _client.GetAsync($"{_urlBaseApi}boletos?{query}");
                    var contentJson = JsonConvert.SerializeObject(boleto);
                    var response = await _client.PostAsync($"{_urlBaseApi}cobrancas/v2/boletos?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                    while (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorString = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorString);
                        var respostaErros = JsonConvert.DeserializeObject<RespostaErros>(errorString);
                        if (respostaErros.erros.Any(x => x.codigo == 4874915))
                        {
                            //Nosso numero duplicado incrementando no BD
                            //await _boletoRepository.AtualizarNossoNumero(nossoNumero);
                            nossoNumero.NUMERO++;
                            boleto.NumeroTituloCliente = "000" + _convenio + Convert.ToString(nossoNumero.NUMERO).PadLeft(10, '0');
                            response = await _client.PostAsync($"{_urlBaseApi}cobrancas/v2/boletos?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadAsStringAsync();
                        //PrintJson(successContent);
                        var detalhes = JsonConvert.DeserializeObject<RespostaRegistroBoletos>(successContent);
                        await _boletoRepository.AtualizarNossoNumero(nossoNumero);
                        //TODO: SALVAR BOLETO NO BD
                        var boletoDB = new SescTO_Boletos
                        {
                            CDELEMENT = boleto.CdElement,
                            SQCOBRANCA = boleto.SqCobranca,
                            CDUOP_CLIENTELA = boleto.Cduop,
                            SQMATRIC_CLIENTELA = boleto.Sqmatric,
                            CONVENIO = Convert.ToInt32(boleto.NumeroConvenio),
                            DATAGERACAO = DateTime.Parse(boleto.DataEmissao),
                            CPF_RESPONSAVEL = Convert.ToString(boleto.Pagador.NumeroInscricao),
                            DATA_VENCIMENTO = DateTime.Parse(boleto.DataVencimento),
                            STATUS = 1,
                            IDCLASSE = "OCRID",
                            VALOR_BOLETO = Convert.ToDecimal(boleto.ValorOriginal),
                            //nossoNumero
                            NOSSO_NUMERO = boleto.NumeroTituloCliente,
                            BASE_NOSSO_NUMERO = nossoNumero.NUMERO--,
                            LINHA_DIGITAVEL = detalhes.LinhaDigitavel,
                            CODIGO_BARRAS = detalhes.CodigoBarraNumerico,
                        };
                        if (boleto.Desconto != null)
                        {
                            boletoDB.VALOR_DESCONTO = Convert.ToDecimal(boleto.Desconto.Valor);
                        }
                        if (boleto.JurosMora != null && boleto.Multa != null)
                        {
                            boletoDB.VALOR_JUROS_MULTA = Convert.ToDecimal(boleto.JurosMora.Valor + boleto.Multa.Valor);
                        }
                        if (detalhes.QrCode != null)
                        {
                            boletoDB.QRCODE_URL = detalhes.QrCode.Url;
                            boletoDB.QRCODE_TXID = detalhes.QrCode.TxId;
                            boletoDB.QRCODE_EMV = detalhes.QrCode.Emv;
                        }
                        await _boletoRepository.SalvarBoleto(boletoDB);
                        return detalhes;
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorContent);
                    return errorContent;
                    //return JsonConvert.DeserializeObject<List<Error>>(errorContent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static void PrintJson(string jsonContent)
        {
            var parsed = JObject.Parse(jsonContent);
            foreach (var pair in parsed)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
        }


        //Integração PIX

        public async Task<dynamic> GerarCobrancaPix(PixCobranca pixCobranca)
        {
            try
            {
                var isCobrancaJaPaga = await _cobrancaRepository.IsCobrancaJaPaga(pixCobranca.IdClasse, pixCobranca.CdElement, pixCobranca.SqCobranca);
                if (isCobrancaJaPaga)
                    return null;

                PixCriado detalhes = null;

                var pixAtivo = await _cobrancaRepository.VerificarCobrancaPixEstaAtivo(pixCobranca.CdElement, pixCobranca.SqCobranca);
                
                if (pixAtivo != null)
                    return pixAtivo;

                else
                {
                    if (token == null)
                    {
                        await ObterToken();
                    }
                    var dados = new[]
                    {
                        new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                    };

                    pixCobranca.Pix.Chave = _chavePix;                 

                    pixCobranca.Pix.Calendario.SetExpiracaoAs23h40m();

                    var formContent = new FormUrlEncodedContent(dados);
                    var query = formContent.ReadAsStringAsync().Result;
                    _client.DefaultRequestHeaders.Remove("Authorization");
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");
                    
                    var contentJson = JsonConvert.SerializeObject(pixCobranca.Pix);
                    var response = await _client.PutAsync($"{_urlBaseApi}pix/v1/cobqrcode/?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadAsStringAsync();
                        detalhes = JsonConvert.DeserializeObject<PixCriado>(successContent);

                        var pixDB = new SescTO_Pix
                        {
                            CDELEMENT = pixCobranca.CdElement,
                            SQCOBRANCA = pixCobranca.SqCobranca,
                            CDUOP = pixCobranca.Cduop,
                            SQMATRIC = pixCobranca.Sqmatric,
                            CRIACAO = detalhes.Calendario.Criacao,
                            EXPIRACAO = detalhes.Calendario.Expiracao,
                            TXID = detalhes.Txid,
                            QRCODE = detalhes.TextoImagemQRcode,
                            LOCATION = detalhes.Location,
                            STATUS = detalhes.Status,
                            IDCLASSE = "OCRID",
                            VALOR_ORIGINAL = Convert.ToDecimal(detalhes.Valor.Original),
                            VALOR_COBRANCA = pixCobranca.Valor,
                            JUROS = pixCobranca.Juros,
                            MULTA = pixCobranca.Multa,
                            DESCONTO = pixCobranca.Desconto,
                            CHAVE = _chavePix,
                            SOLICITACAO = detalhes.SolicitacaoPagador,
                            TIPO = pixCobranca.Tipo
                        };

                        await _pixRepository.SalvarPix(pixDB);                        
                    }                    
                }
                return detalhes;

            }
            catch (Exception e)
            {               
                return null;
            }
        }
        public async Task<dynamic> GerarInscricaoPix(PixCobranca pixCobranca)
        {
            try
            {
                var isPixEventoJaPago = await _pixRepository.IsPixEventoJaPago(pixCobranca.IdClasse, pixCobranca.CdElement, pixCobranca.SqCobranca);
                if (isPixEventoJaPago)
                    return null;

                PixCriado detalhes = null;          
                
                    if (token == null)
                    {
                        await ObterToken();
                    }
                    var dados = new[]
                    {
                        new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                    };
                    pixCobranca.Pix.Chave = _chavePix;
                    pixCobranca.Pix.Calendario.SetExpiracaoAs10m();

                    var formContent = new FormUrlEncodedContent(dados);
                    var query = formContent.ReadAsStringAsync().Result;
                    _client.DefaultRequestHeaders.Remove("Authorization");
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");

                    var contentJson = JsonConvert.SerializeObject(pixCobranca.Pix);
                    var response = await _client.PutAsync($"{_urlBaseApi}pix/v1/cobqrcode/?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadAsStringAsync();
                        detalhes = JsonConvert.DeserializeObject<PixCriado>(successContent);

                        var pixDB = new SescTO_Pix
                        {
                            CDELEMENT = pixCobranca.CdElement,
                            SQCOBRANCA = pixCobranca.SqCobranca,
                            CDUOP = pixCobranca.Cduop,
                            SQMATRIC = pixCobranca.Sqmatric,
                            CRIACAO = detalhes.Calendario.Criacao,
                            EXPIRACAO = detalhes.Calendario.Expiracao,
                            TXID = detalhes.Txid,
                            QRCODE = detalhes.TextoImagemQRcode,
                            LOCATION = detalhes.Location,
                            STATUS = detalhes.Status,
                            IDCLASSE = "OCRID",
                            VALOR_ORIGINAL = Convert.ToDecimal(detalhes.Valor.Original),
                            VALOR_COBRANCA = pixCobranca.Valor,
                            JUROS = pixCobranca.Juros,
                            MULTA = pixCobranca.Multa,
                            DESCONTO = pixCobranca.Desconto,
                            CHAVE = _chavePix,
                            SOLICITACAO = detalhes.SolicitacaoPagador,
                            TIPO = pixCobranca.Tipo
                        };

                        await _pixRepository.SalvarPix(pixDB);
                    }
                
                return detalhes;

            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<dynamic> GerarVendaProdutoPix(PixCobranca pixCobranca)
        {
            try
            {
//                var isPixEventoJaPago = await _pixRepository.IsPixEventoJaPago(pixCobranca.IdClasse, pixCobranca.CdElement, pixCobranca.SqCobranca);
//                if (isPixEventoJaPago)
//                    return null;

                PixCriado detalhes = null;

                if (token == null)
                {
                    await ObterToken();
                }
                var dados = new[]
                {
                        new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                    };
                pixCobranca.Pix.Chave = _chavePix;
                pixCobranca.Pix.Calendario.SetExpiracaoAs10m();

                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");

                pixCobranca.Pix.Devedor = null;
                var contentJson = JsonConvert.SerializeObject(pixCobranca.Pix);

                var response = await _client.PutAsync($"{_urlBaseApi}pix/v1/cobqrcode/?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    detalhes = JsonConvert.DeserializeObject<PixCriado>(successContent);

                    var pixDB = new SescTO_Pix
                    {
                        CDELEMENT = pixCobranca.CdElement,
                        SQCOBRANCA = pixCobranca.SqCobranca,
                        CDUOP = pixCobranca.Cduop,
                        SQMATRIC = pixCobranca.Sqmatric,
                        CRIACAO = detalhes.Calendario.Criacao,
                        EXPIRACAO = detalhes.Calendario.Expiracao,
                        TXID = detalhes.Txid,
                        QRCODE = detalhes.TextoImagemQRcode,
                        LOCATION = detalhes.Location,
                        STATUS = detalhes.Status,
                        IDCLASSE = "VPROD",
                        VALOR_ORIGINAL = Convert.ToDecimal(detalhes.Valor.Original),
                        VALOR_COBRANCA = pixCobranca.Valor,
                        JUROS = pixCobranca.Juros,
                        MULTA = pixCobranca.Multa,
                        DESCONTO = pixCobranca.Desconto,
                        CHAVE = _chavePix,
                        SOLICITACAO = detalhes.SolicitacaoPagador,
                        TIPO = pixCobranca.Tipo
                    };

                    await _pixRepository.SalvarPix(pixDB);
                }

                return detalhes;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<dynamic> GerarRecargaPix(PixRecarga pixRecarga)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }
                var dados = new[]
                {
                        new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                };
                pixRecarga.Pix.Chave = _chavePix;
                pixRecarga.Pix.Calendario.Expiracao = 600; 

                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");
                //var response = await _client.GetAsync($"{_urlBaseApi}boletos?{query}");
                var contentJson = JsonConvert.SerializeObject(pixRecarga.Pix);
                var response = await _client.PutAsync($"{_urlBaseApi}pix/v1/cobqrcode/?{query}", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var detalhes = JsonConvert.DeserializeObject<PixCriado>(successContent);

                    var pixDB = new SescTO_Pix
                    {
                        CDELEMENT = Convert.ToString(pixRecarga.Cartao),
                        SQCOBRANCA = pixRecarga.SqMoviment,
                        CDUOP = pixRecarga.Cduop,
                        SQMATRIC = pixRecarga.Sqmatric,
                        CRIACAO = detalhes.Calendario.Criacao,
                        EXPIRACAO = detalhes.Calendario.Expiracao,
                        TXID = detalhes.Txid,
                        QRCODE = detalhes.TextoImagemQRcode,
                        LOCATION = detalhes.Location,
                        STATUS = detalhes.Status,
                        IDCLASSE = "CART",
                        VALOR_ORIGINAL = Convert.ToDecimal(detalhes.Valor.Original),
                        CHAVE = detalhes.Chave,
                        SOLICITACAO = detalhes.SolicitacaoPagador,
                        TIPO = 2
                    };

                    await _pixRepository.SalvarPix(pixDB);
                    return detalhes;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return errorContent;
                //return JsonConvert.DeserializeObject<List<Error>>(errorContent);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PixCriado> ConsultarPix(string txid)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }

                var dados = new[]
                {
                    new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                };

                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");

                var response = await _client.GetAsync($"{_urlBaseApi}pix/v1/cob/{txid}?{query}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var detalhes = JsonConvert.DeserializeObject<PixCriado>(successContent);
                    return detalhes;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<PixCriado> ConsultarPixPorData(string txid)
        {
            try
            {
                if (token == null)
                {
                    await ObterToken();
                }

                var dados = new[]
                {
                    new KeyValuePair<string, string>("gw-dev-app-key", _appKey)
                };

                var formContent = new FormUrlEncodedContent(dados);
                var query = formContent.ReadAsStringAsync().Result;
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Access_Token}");

                var urls = $"{_urlBaseApi}pix/v1?inicio=2024-10-14T00:00:00Z&fim=2024-10-14T23:59:59Z&paginaAtual=1&itensPorPagina=50&{query}";
                var response = await _client.GetAsync(urls);
                var successContent2 = await response.Content.ReadAsStringAsync();

                string tttt = "";
                for (int i = 1; i <= 19; i++)
                {
                    var urls1 = $"{_urlBaseApi}pix/v1?inicio=2024-10-12T00:00:00Z&fim=2024-10-12T23:59:59Z&paginaAtual={i}&itensPorPagina=50&{query}";

                    var response1 = await _client.GetAsync(urls1);
                    var successContent21 = await response1.Content.ReadAsStringAsync();

                    tttt = tttt + " ### " + successContent21;

                }


                // var response = await _client.GetAsync($"{_urlBaseApi}pix/v1/cob/{txid}?{query}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var detalhes = JsonConvert.DeserializeObject<PixCriado>(successContent);
                    return detalhes;
                }

                //var errorContent = await response.Content.ReadAsStringAsync();
                //return JsonConvert.DeserializeObject<DetalhamentoBoleto>(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}