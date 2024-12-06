using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SiteSesc.Data;
using SiteSesc.Helpers;
using SiteSesc.Models;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.ApiPagamento.Cielo;
using SiteSesc.Models.ApiPagamento.Cielo.ViewModel;
using SiteSesc.Models.ApiPagamento.Pix;
using SiteSesc.Models.ApiPagamentoV2;
using SiteSesc.Models.DB2;
using SiteSesc.Models.Enums;
using SiteSesc.Models.ViewModel;
using SiteSesc.Repositories;
using SiteSesc.Services;

namespace SiteSesc.Areas.Cliente.Controllers
{
    [Authorize]
    [Area("Cliente")]
    [Route("cliente/cobrancas")]
    //[Route("cobrancas")]
    public class cobrancaController : BaseClienteController
    {
        private readonly SiteSescContext _context;
        private IWebHostEnvironment _env;
        private readonly IConfiguration configuration;
        public readonly HostConfiguration hostConfiguration;
        private IMemoryCache cache;
        private Usuario usuario;
        private bool devMode;
        private readonly ClienteRepository _clienteRepository;
        private SolicitacaoCadastroRepository _solicitacaoRepository;
        private CobrancaRepository _cobrancaRepository;
        private readonly SafeExecutor _safeExecutor;
        private readonly EmailService _emailService;
        private readonly ApiPagamentoV2Service _apiPagamentoV2Service;


        public cobrancaController(SiteSescContext context,
            IWebHostEnvironment env,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            UsuarioRepository usuarioRepository,
            SolicitacaoCadastroRepository solicitacaoCadastroRepository,
            ClienteRepository clienteRepository,
            CobrancaRepository cobrancaRepository,
            SafeExecutor safeExecutor,
            EmailService emailService,
            ApiPagamentoV2Service apiPagamentoV2Service)
        {
            this.configuration = configuration;
            cache = memoryCache;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _context = context;
            _env = env;
            _usuarioRepository = usuarioRepository;
            _solicitacaoRepository = solicitacaoCadastroRepository;
            _clienteRepository = clienteRepository;
            _safeExecutor = safeExecutor;
            _cobrancaRepository = cobrancaRepository;
            _emailService = emailService;
            _apiPagamentoV2Service = apiPagamentoV2Service;
        }

        [Route("atividade/{cdelement}/{cpf}/{inscricao?}")]
        //[Route("atividade")]
        public async Task<IActionResult> CobrancasPorAtividade(string cdelement, string cpf, bool? inscricao = false)
        {
            var listaCobrancas = new List<CobrancaHistorico>();
            var turma = Util.CdelementToTurma(cdelement);
            var clienteCentral = await _clienteRepository.ObterClientePorCpf(cpf);
            var cobrancas = await _cobrancaRepository.ObterProximasCobrancasPorCpf(cpf);
            cobrancas = cobrancas?.Where(c => c.cdelement == cdelement && c.strecebido == 0).ToList();
            if (inscricao == true)
            {
                if (cobrancas.Any())
                    cobrancas = cobrancas.OrderByDescending(a => a.sqcobranca).Take(1).ToList();
            }
            ViewBag.Cobrancas = cobrancas.Where(c => c.cdelement == cdelement && c.strecebido == 0).ToList();
            return View(clienteCentral);
        }

        [Route("todas")]
        public async Task<IActionResult> CobrancasPorCpf(string? cpfDependente = null)
        {
            //Obtem as cobranças dos ultimos 5 anos
            var anoBase = DateTime.Now.Year - 5;
            var cpfBusca = cpf;
            if (!string.IsNullOrEmpty(cpfDependente))
                cpfBusca = cpfDependente;

            var cobrancas = new List<COBRANCA>();

            for (int i = anoBase; i <= DateTime.Now.Year; i++)
            {
                cobrancas.AddRange(await _cobrancaRepository.ObterCobrancaPorCpf(cpfBusca, i));
            }

            ViewBag.Cliente = await _clienteRepository.ObterClientePorCpf(cpfBusca);
            return View(cobrancas);
        }


        [HttpPost]
        [Route("gerar-pix")]
        public async Task<IActionResult> GeraPix(DadosCobranca dados, decimal? valorRecarga = null)
        {
            var clienteCentral = await _clienteRepository.ObterClientePorCpf(dados.CPF);
           // var responsavel = await _clienteRepository.ObterResponsavel(clienteCentral.Cduop, clienteCentral.Sqmatric);
            //var cpfPagador = responsavel.Count() > 0 ? responsavel.FirstOrDefault().CPF : clienteCentral.Nucpf;
            //var nomePagador = responsavel.Count() > 0 ? responsavel.FirstOrDefault().NOME : clienteCentral.Nmcliente.Trim();
            int tipo = Util.GetTipoPix(dados.IDCLASSE);
            if (clienteCentral != null)
            {
                if (tipo == (int)TipoPix.Cobranca)
                {
                    var cobranca = new COBRANCA(dados.IDCLASSE, dados.CDELEMENT, dados.SQCOBRANCA, 0);
                    var cobrancaAtualizada = await _cobrancaRepository.ObterValorAtualizado(cobranca);
                    if (cobrancaAtualizada != null)
                    {
                        cobrancaAtualizada.atividade = cobrancaAtualizada.atividade.Replace("\"", "");

                        var request = new CobrancaPixRequest
                        {
                            Cpf = dados.CPF,
                            CdUop = dados.CDUOP,
                            SqMatric = dados.SQMATRIC,
                            CdElement = cobrancaAtualizada.cdelement,
                            SqCobranca = cobrancaAtualizada.sqcobranca,
                            ValorRecebido = cobrancaAtualizada.valorRecebido,
                            ValorJuros = cobrancaAtualizada.jurosMora,
                            ValorDesconto = cobrancaAtualizada.descontoConcedido,
                            ValorAcresimo = cobrancaAtualizada.jurosMora + cobrancaAtualizada.multa,
                            NumCartao = clienteCentral.Numcartao,
                            Pix = new PixRequest 
                            {
                                Nome = clienteCentral.Nmcliente,
                                Cpf = clienteCentral.Nucpf,
                                Valor = cobrancaAtualizada.valorRecebido,
                                DescricaoPagamento = cobrancaAtualizada.atividade
                            }
                        };

                        var getPix = await _apiPagamentoV2Service.CobrancaPixCriarAsync(request);

                       // var getPix = await _cobrancaRepository.GetPix(new PixCobranca(cobrancaAtualizada, clienteCentral, tipo, cpfPagador, nomePagador));
                        if (getPix.IsSuccess)
                        {
                            var imagemPix = Util.GerarQrCode(getPix.Content.PixCopiaECola);
                            return PartialView(new PixViewModel(cobrancaAtualizada, imagemPix, getPix.Content.PixCopiaECola));
                        }
                    }
                }
                else if (tipo == (int)TipoPix.Recarga)
                {
                    if (valorRecarga != null && valorRecarga > 0)
                    {
                        var request = new RecargaPixRequest
                        {
                            Nome = clienteCentral.Nmcliente,
                            Cpf = clienteCentral.Nucpf,
                            NumCartao = clienteCentral.Numcartao,
                            Sqmatric = clienteCentral.Sqmatric,
                            Cduop = clienteCentral.Cduop,
                            Valor = Convert.ToDecimal(valorRecarga),
                            DescricaoPagamento = "Recarga Pix Credencial Sesc"
                        };

                        var getPix = await _apiPagamentoV2Service.RecargaPixCriarAsync(request);

                        //var getPix = await _cobrancaRepository.GetPixRecarga(new PixRecarga(valorRecarga, cpfPagador, nomePagador, clienteCentral));
                        if (getPix.IsSuccess)
                        {
                            var imagemPix = Util.GerarQrCode(getPix.Content.PixCopiaECola);
                            return PartialView(new PixViewModel(getPix.Content.Valor, "Recarga Pix Credencial", getPix.Content.DataCriacao, imagemPix, getPix.Content.PixCopiaECola));
                        }
                    }

                }
            }
            return PartialView();
        }

        [HttpPost("pagamento-credito")]
        public async Task<IActionResult> PagamentoCartao(DadosCobranca dados, PaymentViewModel payment)
        {
            try
            {
                if (dados.IDCLASSE == "CART")
                {
                    var recarga = new CartaoCredito(dados, payment);
                    var retornoRecarga = await _cobrancaRepository.Recarga(recarga);
                    if (retornoRecarga != null)
                    {
                        if (retornoRecarga != "Cartão recarregado com sucesso.")
                        {
                            var response = JsonConvert.DeserializeObject(retornoRecarga);
                            if (response is ApiResponse)
                            {
                                if (response.Success == false)
                                {
                                    return Json(new
                                    {
                                        code = 1,
                                        msg = response.Message
                                    });
                                }
                            }
                        }

                        var usuario = await _usuarioRepository.GetUsuarioCpf(dados.CPF);
                        if (!string.IsNullOrEmpty(usuario.Email))
                        {
                            var numeroCartao = recarga.Payment.CreditCard.CardNumber;
                            var cartaoFormatado = "xxxx.xxxx.xxxx." + numeroCartao.Substring(numeroCartao.Length - 4);

                            Dictionary<string, string> listaParametrosEmail = new Dictionary<string, string>
                            {
                                { "[[nomeCliente]]", usuario.Nome },
                                { "[[dataRecarga]]", DateTime.Now.ToString() },
                                { "[[titular]]", payment.Name },
                                { "[[numero]]", cartaoFormatado },
                                { "[[bandeira]]", Util.IdentificarBandeira(payment.CardNumber) },
                                { "[[valor]]", payment.Amount.ToString("C") }
                            };

                            var bodyEmail = BuildTemplateEmail.Make(_env, "\\templates\\templates-email\\confirmacao_recarga.html", listaParametrosEmail);
                            var enviaEmail = _emailService.Send(usuario.Email, bodyEmail, "Pagamento recebido");
                        }
                        return Ok(new { code = 1, msg = "Pagamento realizado" });
                    }

                    return Ok(new { code = 0, msg = "Falha ao processar pagamento" });
                }
                else if (dados.IDCLASSE == "OCRID")
                {
                    var cobranca = new COBRANCA(dados.IDCLASSE, dados.CDELEMENT, dados.SQCOBRANCA, 0);
                    var cobrancaAtualizada = await _cobrancaRepository.ObterValorAtualizado(cobranca);

                    if (cobrancaAtualizada != null)
                    {
                        /*
                        var cobrancaValor = new CobrancaValor
                        {
                            IDCLASSE = cobrancaAtualizada.idclasse,
                            CDELEMENT = cobrancaAtualizada.cdelement,
                            SQCOBRANCA = cobrancaAtualizada.sqcobranca,
                            ValorOriginal = cobrancaAtualizada.valorOriginal,
                            ValorRecebido = cobrancaAtualizada.valorRecebido,
                            JurosMora = cobrancaAtualizada.jurosMora,
                            Multa = cobrancaAtualizada.multa,
                            OutrosRecebimentos = cobrancaAtualizada.outrosRecebimentos,
                            DescontoConcedido = cobrancaAtualizada.descontoConcedido
                        };*/
                       // payment.Amount = cobrancaValor.ValorRecebido;
                       // var transacao = new Transacao(payment, cobrancaValor);
                       // var efetuaPagamento = await _cobrancaRepository.PagamentoCartao(transacao);


                        var request = new CobrancaCartaoCieloRequest
                        {
                            Cpf = dados.CPF,
                            CdUop = dados.CDUOP,
                            SqMatric = dados.SQMATRIC,                            
                            CdElement = cobrancaAtualizada.cdelement,
                            SqCobranca = cobrancaAtualizada.sqcobranca,
                            ValorRecebido = cobrancaAtualizada.valorRecebido,
                            ValorJuros = cobrancaAtualizada.jurosMora,
                            ValorDesconto = cobrancaAtualizada.descontoConcedido,
                            ValorAcresimo = cobrancaAtualizada.jurosMora + cobrancaAtualizada.multa,                            
                            CartaoCielo = new CartaoCieloRequest
                            {
                                Nome = payment.Name,
                                Cpf = payment.Identity,
                                NumeroCartao = payment.CardNumber,
                                DataExpiracao = payment.ExpirationDate,
                                CodigoSeguranca = payment.SecurityCode,
                                Valor = payment.Amount,
                                Bandeira = "Visa"                            
                            }
                        
                        };

                        var efetuaPagamento = await _apiPagamentoV2Service.CobrancaCartaoCieloAsync(request);

                       // if (efetuaPagamento == "Cobrança paga com sucesso.")
                        if (efetuaPagamento.IsSuccess)
                        {
                            return Json(new
                            {
                                Code = 1,
                                Validacao = "Pagamento realizado com sucesso.",
                                ClassAlert = "success"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Code = 0,
                                Validacao = "Falha ao processar o pagamento. Revise os dados do cartão e tente novamente.",
                                ClassAlert = "error"
                            });
                        }
                    }


                    return null;
                }
            }
            catch (Exception e)
            {
                return Ok(new { code = 0, msg = "Falha ao processar pagamento" });

            }
            return Ok(new { code = 0, msg = "Falha ao processar pagamento" });
        }

        [Route("partialview-credito/{cpf}/{cduop}/{sqmatric}/{idclasse}/{cdelement}/{sqcobranca}")]
        public async Task<IActionResult> PartialViewCredito(string cpf, int cduop, int sqmatric, string idclasse, string cdelement, short sqcobranca)
        {


            var dados = new DadosCobranca
            {
                CPF = cpf.Trim(),
                CDUOP = cduop,
                SQMATRIC = sqmatric,
                IDCLASSE = idclasse,
                CDELEMENT = cdelement,
                SQCOBRANCA = sqcobranca
            };

            if (idclasse == "OCRID")
            {
                var cobranca = new COBRANCA(idclasse, cdelement, sqcobranca, 0);
                var cobrancaAtualizada = await _cobrancaRepository.ObterValorAtualizado(cobranca);
                if (cobrancaAtualizada != null)
                {
                    dados.VALOR = cobrancaAtualizada.valorRecebido.ToString("C");
                    dados.DESCRICAO = $"{cobrancaAtualizada.atividade} - Venc.: {cobrancaAtualizada.vencimento.ToShortDateString()}";
                }
            }
            return PartialView("_PartialViewCartaoCredito", dados);
        }

    }
}
