using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using PagamentoApi.ApiAutenticacoes;
using PagamentoApi.Clients;
using PagamentoApi.DTOs;

using PagamentoApi.Models.HabilitaTransacoes;
using PagamentoApi.Models.Pix;
using PagamentoApi.Repositories;
using PagamentoApi.Settings;
using PagamentoApi.SignalR;

namespace PagamentoApi.Controllers
{/*
    
    [Route("v1/[controller]")]
    [ApiController]

    public class PixController : ControllerBase
    {
        private readonly BBApiSettings _apiSettings;
        private readonly string _chavePix;
        private IWebHostEnvironment _env;

        private readonly IHubContext<WebHookHub> _hubContext;
        private readonly ClienteConectado _clienteConectados;
        public PixController(BBApiSettings apiSettings, IWebHostEnvironment env, IHubContext<WebHookHub> hubContext, ClienteConectado clienteConectados)
        {
            _apiSettings = apiSettings;
            _env = env;
            if (_apiSettings.Sandbox)
            {
                //desenvolvimento
                _chavePix = _apiSettings.ChavePixSandbox;
            }
            else
            {
                //produção
                _chavePix = _apiSettings.ChavePix;
            }
            _hubContext = hubContext;
            _clienteConectados = clienteConectados;
        }

        [HttpPost("gerar")]
        [Authorize]
        public async Task<dynamic> GerarPix([FromServices] BBClientPixV2 bbClient, [FromServices] CobrancaRepository cobrancaRepository, [FromBody] PixCobranca pixCobranca)
        {
            if (!HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));

            if (pixCobranca.Tipo == 1 && pixCobranca.CdElement.Length < 24)
            {
                return BadRequest(pixCobranca);
            }
            var cobrancaEstaPaga = await cobrancaRepository.VerificarCobrancaEstaPaga(pixCobranca.CdElement, pixCobranca.SqCobranca);

            if (cobrancaEstaPaga)
                return BadRequest("Cobrança Já Pagar");

            pixCobranca.Pix.SolicitacaoPagador = Regex.Replace(pixCobranca.Pix.SolicitacaoPagador, "[^\\w\\s]", "");

            var obterCobranca = await cobrancaRepository.ObterCobranca(pixCobranca.IdClasse, pixCobranca.CdElement, pixCobranca.SqCobranca);
            if(obterCobranca != null)
            {
                var descricao = pixCobranca.Pix.SolicitacaoPagador + " - REF: " + obterCobranca.Vencimento.ToString("dd/MM/yyyy");
                pixCobranca.Pix.SolicitacaoPagador = descricao.Length > 100 ? descricao.Substring(0, 100) : descricao;
            }

            var pixCriado = await bbClient.GerarCobrancaPix(pixCobranca);
            if (pixCriado == null)
            {
                return BadRequest(pixCriado);
            }

            return pixCriado;
        }

        [HttpPost("inscricao")]
        [Authorize]
        public async Task<dynamic> GerarInscricaoPix([FromServices] BBClient bbClient, [FromBody] PixCobranca pixCobranca)
        {
            if (!HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));

            if (pixCobranca.Tipo != 3)
            {
                return null;
            }

            pixCobranca.Pix.SolicitacaoPagador = Regex.Replace(pixCobranca.Pix.SolicitacaoPagador, "[^\\w\\s]", "");
            var pixCriado = await bbClient.GerarInscricaoPix(pixCobranca);
            if (pixCriado == null)
            {
                return null;
            }

            return pixCriado;
        }

        [HttpPost("vendaproduto")]
        [Authorize]
        public async Task<dynamic> GerarVendaProdutoPix([FromServices] BBClient bbClient, [FromBody] PixCobranca pixCobranca)
        {

            if (!HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));

            if (pixCobranca.Tipo != 4)
            {
                return null;
            }

            pixCobranca.Pix.SolicitacaoPagador = Regex.Replace(pixCobranca.Pix.SolicitacaoPagador, "[^\\w\\s]", "");
            var pixCriado = await bbClient.GerarVendaProdutoPix(pixCobranca);
            if (pixCriado == null)
            {
                return null;
            }

            return pixCriado;
        }

        [HttpPost("recarga")]
        [Authorize]
        public async Task<dynamic> RecargaPix([FromServices] BBClientPixV2 bbClient, [FromBody] PixRecarga pixRecarga)
        {
            if (pixRecarga.Pix.Valor.Original > 150)
                return BadRequest("Desculpe, não foi possível prosseguir. O valor o máximo da recarga é R$ 150,00");

            if (!HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));

            var pixCriado = await bbClient.GerarRecargaPix(pixRecarga);

            if (pixCriado == null)
                return BadRequest(pixCriado);
            
            return pixCriado;
        }

        [HttpGet("consultar/{txid}")]
        [Authorize]
        public async Task<dynamic> ConsultarPixPorTxId([FromServices] BBClient bbClient, string txid)
        {
            return await bbClient.ConsultarPix(txid);
        }

        [HttpGet("verificapix/{txid}")]
        [Authorize]
        public async Task<dynamic> VerificaPixPago([FromServices] PixRepository pixRepository, string txid)
        {
            return await pixRepository.VerificaPixPago(txid);
        }

        [HttpGet("listar")]
        [Authorize]
        public async Task<dynamic> ListarPix([FromServices] PixRepository pixRepository)
        {
            return await pixRepository.ListarPix();
        }

        [HttpGet("atualizar")]
        [Authorize]
        public async Task<dynamic> AtualizarPixAtivos([FromServices] BBClient bbClient, [FromServices] PixRepository pixRepository)
        {
            return await pixRepository.AtualizarPixAtivos(bbClient);
        }

        [HttpGet("consultarPorTxid/{txId}")]
        public async Task<dynamic> consultarPorTxid([FromServices] BBClient bbClient, [FromServices] PixRepository pixRepository, [FromServices] IApiAutenticacao apiAutenticacao, string txId)
        {
            if (_env.IsDevelopment())
            {
                return await bbClient.ConsultarPix(txId);
            }


            if (string.IsNullOrEmpty(txId))
                return false;

            string? apiKey = Request.Headers[Constants.ApiChaveHeaderNome];

            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest();

            bool isValido = apiAutenticacao.IsValidoChaveApi(apiKey);

            if (!isValido)
                return Unauthorized();

            return await bbClient.ConsultarPix(txId);
        }

        [HttpGet("consultarPorData/{txId}")]
        public async Task<dynamic> ConsultarPixPorData([FromServices] BBClient bbClient, [FromServices] PixRepository pixRepository, [FromServices] IApiAutenticacao apiAutenticacao, string txId)
        {
            if (_env.IsDevelopment())
            {
                return await bbClient.ConsultarPixPorData(txId);
            }


            if (string.IsNullOrEmpty(txId))
                return false;

            string? apiKey = Request.Headers[Constants.ApiChaveHeaderNome];

            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest();

            bool isValido = apiAutenticacao.IsValidoChaveApi(apiKey);

            if (!isValido)
                return Unauthorized();

            return await bbClient.ConsultarPix(txId);
        }

        [HttpGet("AtualizaPixWebHook/{txId}")]
        public async Task<dynamic> AtualizaPix([FromServices] BBClient bbClient, [FromServices] PixRepository pixRepository, [FromServices] IApiAutenticacao apiAutenticacao, string txId)
        {
         

            if (string.IsNullOrEmpty(txId))
                return false;

            string? apiKey = Request.Headers[Constants.ApiChaveHeaderNome];

            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest();

            bool isValido = apiAutenticacao.IsValidoChaveApi(apiKey);

            if (!isValido)
                return Unauthorized();

            var verificaPix = await pixRepository.AtualizarPixAtivos(bbClient, txId);
            if (verificaPix)
            {
                var cliente = _clienteConectados.Clientes.Find(x => x.TxId == txId);
                if (cliente != null)
                {
                    cliente.TxId = null;
                    new Thread(async () =>
                    {
                        await _hubContext.Clients.Client(cliente.ConnectionId).SendAsync("confirmacaoPagamento", verificaPix);
                    }).Start();
                }
            }
            return verificaPix;
        }
    }
    */
    
}