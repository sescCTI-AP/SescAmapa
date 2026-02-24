using Newtonsoft.Json;
using PagamentoApi.Repositories;
using PagamentoApi.Settings;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using PagamentoApi.Models.Pix;
using PagamentoApi.V2.Requests;
using PagamentoApi.V2.Responses;

namespace PagamentoApi.Clients
{
    public class BBClientPixV2
    {
        private readonly HttpClient _client;
        private readonly BBApiPixV2Settings _settings;
        private readonly PixRepository _pixRepository;
        private readonly CobrancaRepository _cobrancaRepository;

        public BBClientPixV2(HttpClient client, BBApiPixV2Settings settings, PixRepository pixRepository, CobrancaRepository cobrancaRepository)
        {
            _client = client;
            _settings = settings;
            _pixRepository = pixRepository;
            _cobrancaRepository = cobrancaRepository;
           
        }

        public async Task<dynamic> GerarRecargaPix(PixRecarga pixRecarga)
        {
            try
            {
                var pixRequest = new CriarPixRequest {
                    DevedorNome = pixRecarga.Pix.Devedor.Nome,
                    DevedorCpfCnpj = pixRecarga.Pix.Devedor.Cpf,
                    ValorOriginal = pixRecarga.Pix.Valor.Original,
                    CalendarioExpiracao = 600,
                    solcnpjitacaoPagador = pixRecarga.Pix.SolicitacaoPagador                
                };

                string jsonContent = System.Text.Json.JsonSerializer.Serialize(pixRequest);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("client_id", _settings.ClientId);
                    httpClient.DefaultRequestHeaders.Add("client_secret", _settings.ClientSecret);

                    var conteudo = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage resposta = await httpClient.PostAsync(_settings.UrlApi + "pix/v2/cob", conteudo);

                    if (resposta.IsSuccessStatusCode)
                    {
                        var successContent = await resposta.Content.ReadAsStringAsync();
                        var detalhes = JsonConvert.DeserializeObject<ResponseResult<PixResponse>>(successContent);
                        var pixResponse = detalhes.Content;

                        PixCriado pixCriado = new PixCriado();
                        PixCalendario calendario = new PixCalendario { Criacao = pixResponse.Criacao, Expiracao = pixResponse.Expiracao };

                        pixCriado.TextoImagemQRcode = pixResponse.PixCopiaECola;
                        pixCriado.Calendario = calendario;
                        pixCriado.Txid = pixResponse.Txid;
                        pixCriado.Location = pixResponse.Location;
                        pixCriado.Status = pixResponse.Situacao;
                        pixCriado.Valor = new PixValor { Original = pixResponse.ValorOriginal } ;
                        pixCriado.Chave = pixResponse.Chave;
                        pixCriado.SolicitacaoPagador = pixResponse.SolicitacaoPagador;

                        var pixDB = new SescTO_Pix
                        {
                            CDELEMENT = Convert.ToString(pixRecarga.Cartao),
                            SQCOBRANCA = pixRecarga.SqMoviment,
                            CDUOP = pixRecarga.Cduop,
                            SQMATRIC = pixRecarga.Sqmatric,
                            CRIACAO = pixCriado.Calendario.Criacao,
                            EXPIRACAO = pixCriado.Calendario.Expiracao,
                            TXID = pixCriado.Txid,
                            QRCODE = pixCriado.TextoImagemQRcode,
                            LOCATION = pixCriado.Location,
                            STATUS = pixCriado.Status,
                            IDCLASSE = "CART",
                            VALOR_ORIGINAL = Convert.ToDecimal(pixCriado.Valor.Original),
                            CHAVE = pixCriado.Chave,
                            SOLICITACAO = pixCriado.SolicitacaoPagador,
                            TIPO = 2
                        };
                        await _pixRepository.SalvarPix(pixDB);                        
                        return pixCriado;
                    }
                }
                
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> GerarCobrancaPix(PixCobranca pixCobranca)
        {
            try
            {
                var isCobrancaJaPaga = await _cobrancaRepository.IsCobrancaJaPaga(pixCobranca.IdClasse, pixCobranca.CdElement, pixCobranca.SqCobranca);
                if (isCobrancaJaPaga)
                    return null;

                var pixAtivo = await _cobrancaRepository.VerificarCobrancaPixEstaAtivo(pixCobranca.CdElement, pixCobranca.SqCobranca);

                if (pixAtivo != null)
                    return pixAtivo;

                pixCobranca.Pix.Calendario.SetExpiracaoAs23h40m();
                var pixRequest = new CriarPixRequest
                {                   
                    DevedorNome = pixCobranca.Pix.Devedor.Nome,
                    DevedorCpfCnpj = pixCobranca.Pix.Devedor.Cpf,
                    ValorOriginal = pixCobranca.Pix.Valor.Original,
                    CalendarioExpiracao = pixCobranca.Pix.Calendario.Expiracao,
                    solcnpjitacaoPagador = pixCobranca.Pix.SolicitacaoPagador
                };
                
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(pixRequest);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("client_id", _settings.ClientId);
                    httpClient.DefaultRequestHeaders.Add("client_secret", _settings.ClientSecret);

                    var conteudo = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage resposta = await httpClient.PostAsync(_settings.UrlApi + "pix/v2/cob", conteudo);

                    if (resposta.IsSuccessStatusCode)
                    {
                        var successContent = await resposta.Content.ReadAsStringAsync();
                        var detalhes = JsonConvert.DeserializeObject<ResponseResult<PixResponse>>(successContent);
                        var pixResponse = detalhes.Content;
                        
                        PixCriado pixCriado = new PixCriado();
                        PixCalendario calendario = new PixCalendario { Criacao = pixResponse.Criacao, Expiracao = pixResponse.Expiracao };

                        pixCriado.TextoImagemQRcode = pixResponse.PixCopiaECola;
                        pixCriado.Calendario = calendario;
                        pixCriado.Txid = pixResponse.Txid;
                        pixCriado.Location = pixResponse.Location;
                        pixCriado.Status = pixResponse.Situacao;
                        pixCriado.Valor = new PixValor { Original = pixResponse.ValorOriginal } ;
                        pixCriado.Chave = pixResponse.Chave;
                        pixCriado.SolicitacaoPagador = pixResponse.SolicitacaoPagador;
                        
                        var pixDB = new SescTO_Pix
                        {
                            CDELEMENT = pixCobranca.CdElement,
                            SQCOBRANCA = pixCobranca.SqCobranca,
                            CDUOP = pixCobranca.Cduop,
                            SQMATRIC = pixCobranca.Sqmatric,
                            CRIACAO = pixCriado.Calendario.Criacao ,
                            EXPIRACAO = pixCriado.Calendario.Expiracao,
                            TXID = pixCriado.Txid,
                            QRCODE = pixCriado.TextoImagemQRcode,
                            LOCATION = pixCriado.Location,
                            STATUS = pixCriado.Status,
                            IDCLASSE = "OCRID",
                            VALOR_ORIGINAL = Convert.ToDecimal(pixCriado.Valor.Original),
                            VALOR_COBRANCA = pixCobranca.Valor,
                            JUROS = pixCobranca.Juros,
                            MULTA = pixCobranca.Multa,
                            DESCONTO = pixCobranca.Desconto,
                            CHAVE = pixCriado.Chave,
                            SOLICITACAO = pixCriado.SolicitacaoPagador,
                            TIPO = pixCobranca.Tipo
                        };

                        await _pixRepository.SalvarPix(pixDB);
                        return pixCriado;
                    }                  
                }
                return null;
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
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("client_id", _settings.ClientId);
                    httpClient.DefaultRequestHeaders.Add("client_secret", _settings.ClientSecret);

                    HttpResponseMessage resposta = await httpClient.GetAsync(_settings.UrlApi + $"pix/v2/cob/{txid}");

                    if (resposta.IsSuccessStatusCode)
                    {
                        var successContent = await resposta.Content.ReadAsStringAsync();
                        var detalhes = JsonConvert.DeserializeObject<ResponseResult<PixCriado>>(successContent);
                        var pixCriado = detalhes.Content;

                        pixCriado.TextoImagemQRcode = pixCriado.PixCopiaECola;

                        return pixCriado;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;

            }
        }
    }
}
