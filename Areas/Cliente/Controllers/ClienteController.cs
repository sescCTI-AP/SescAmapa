using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SiteSesc.Controllers;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.DB2;
using SiteSesc.Models.Enums;
using SiteSesc.Models.ViewModel;
using SiteSesc.Repositories;
using SiteSesc.Services;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SiteSesc.Areas.Cliente.Controllers
{
    [Authorize]
    [Area("Cliente")]
    [Route("cliente")]
    public class ClienteController : BaseClienteController
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
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly SafeExecutor _safeExecutor;
        private readonly AtividadeOnLineReposotory _atividadeOnLineRepository;

        public ClienteController(SiteSescContext context,
            IWebHostEnvironment env,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            UsuarioRepository usuarioRepository,
            SolicitacaoCadastroRepository solicitacaoCadastroRepository,
            ClienteRepository clienteRepository,
            SafeExecutor safeExecutor,
            CobrancaRepository cobrancaRepository,
            AtividadeOnLineReposotory atividadeOnLineRepository)
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
            _atividadeOnLineRepository = atividadeOnLineRepository;
        }

        public async Task<IActionResult> Index(string? cpfDependente = null)
        {
            #region Busca dados do cliente
            var clienteCentral = new ClienteCentral();

            if (!string.IsNullOrEmpty(cpfDependente))
            {
                var titular = await _clienteRepository.ObterClientePorCpf(cpf);
                var dependente = await _clienteRepository.ObterClientePorCpf(cpfDependente);
                if (titular != null && dependente != null)
                {
                    if (dependente.Cduotitul != titular.Cduop && dependente.Sqtitulmat != titular.Sqmatric)
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                cpf = cpfDependente;
            }

            clienteCentral = await _clienteRepository.ObterClientePorCpf(cpf);
            if (clienteCentral == null)
                return RedirectToAction("NovoCliente");

            if (clienteCentral.Numcartao != null)
                ViewBag.Movimentacao = await _clienteRepository.ObterMovimentacaoCartao(Convert.ToInt32(clienteCentral.Numcartao));

            var usuario = await _usuarioRepository.ObterUsuarioPorCpf(cpf);
            if (usuario != null)
            {
                usuario.Senha = "";

                //var dataInicioRematricula = new DateTime(2024, 11, 1);
                //var dataFimRematricula = new DateTime(2024, 11, 30);

                //if (usuario.IdPerfilUsuario != (int)PerfilUsuarioEnum.Cliente)
                //{
                //    var telaInicial = await _usuarioRepository.VerificaTelaInicial(Convert.ToInt32(idUsuario));
                //    ViewBag.TelaInicial = telaInicial != null ? telaInicial : "/admin/dashboard";
                //}
            }
            #endregion

            TimeSpan diferenca = clienteCentral.Dtvencto - DateTime.Now;
            ViewBag.Renovacao = diferenca.TotalDays <= 30;
            ViewBag.Correcao = await _solicitacaoRepository.GetSoliticacaoCorrecao(cpf);
            ViewBag.HasSolicitacao = _solicitacaoRepository.HasSolicitacaoAnaliseCliente(cpf);

            // Anonimizar CPF do cliente antes de passar para a View
            ViewBag.Usuario = usuario;
            ViewBag.TipoCategoria = await _clienteRepository.ObtemTipoCategoria(clienteCentral.Cdcategori);

            return View(clienteCentral);
        }


        public static class AnonymizationHelper
        {
            public static string AnonymizeCpf(string cpf)
            {
                if (string.IsNullOrEmpty(cpf) || cpf.Length < 11) return cpf; // Retorna o valor original se for nulo ou curto
                return cpf.Substring(0, 3) + ".***.***-" + cpf.Substring(9, 2);
            }

            public static string AnonymizeRg(string rg)
            {
                if (string.IsNullOrEmpty(rg) || rg.Length < 8) return rg; // Retorna o valor original se for nulo ou curto
                return rg.Substring(0, 2) + "***" + rg.Substring(5, 3);
            }
        }


        [Route("movimentacao-cartao/{numcartao}")]
        public async Task<IActionResult> MovimentacaoCartao(string? numcartao = null)
        {
            if (!string.IsNullOrEmpty(numcartao))
                cpf = numcartao;

            var movimentacao = await _clienteRepository.ObterMovimentacaoCartao(Convert.ToInt32(numcartao));
            return Ok(movimentacao);
        }

        [Route("contratos")]
        public async Task<IActionResult> Contratos()
        {
            return View();
        }

        [Route("novo-cliente")]
        public async Task<IActionResult> NovoCliente()
        {
            var usuario = await _usuarioRepository.GetUsuario(new Guid(guid));
            ViewBag.HasSolicitacao = _solicitacaoRepository.HasSolicitacaoAnaliseCliente(cpf);
            return View(usuario);
        }

        [HttpGet("get-dependentes")]
        public async Task<IActionResult> GetDependentes(string? cpfDependente = null)
        {
            if (!string.IsNullOrEmpty(cpfDependente))
                cpf = cpfDependente;

            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var clientes = await _clienteRepository.ObterClienteDependentesPorCpf(cpf);
                var dependentes = clientes.Count() > 1 ? clientes.Skip(1).ToArray() : null;
                return Ok(dependentes);
            });
        }



        [HttpGet("get-credencial/{cpfDependente?}")]
        public async Task<IActionResult> GetCredencial(string? cpfDependente = null)
        {
            if (!string.IsNullOrEmpty(cpfDependente))
                cpf = cpfDependente;

            return await _safeExecutor.ExecuteSafe(async () =>
            {
                string credencial = string.Format("data:image/png;base64,{0}", await _clienteRepository.ObtemCredencial(cpf));
                return Ok(credencial);
            });
        }

        [HttpGet("get-saldo/{cpfDependente?}")]
        public async Task<IActionResult> GetSaldo(string? cpfDependente = null)
        {
            if (!string.IsNullOrEmpty(cpfDependente))
                cpf = cpfDependente;

            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var saldo = await _clienteRepository.ObterSaldoCartao(cpf);
                return Ok(saldo);
            });
        }

        [HttpGet("get-matriculas/{cpfDependente?}")]
        public async Task<IActionResult> GetMatriculas(string? cpfDependente = null)
        {
            if (!string.IsNullOrEmpty(cpfDependente))
                cpf = cpfDependente;

            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var matriculas = await _clienteRepository.GetMatriculas(cpf);
                return Ok(matriculas);
            });
        }

        [HttpGet("get-cobrancas/{cpfDependente?}")]
        public async Task<IActionResult> GetCobrancas(string? cpfDependente = null)
        {
            if (!string.IsNullOrEmpty(cpfDependente))
                cpf = cpfDependente;

            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var proximasCobrancas = await _cobrancaRepository.ObterProximasCobrancasPorCpf(cpf);
                return Ok(proximasCobrancas);
            });
        }

        [Route("dependente/{cpf}")]
        public async Task<IActionResult> Dependente(string cpf)
        {
            if (!string.IsNullOrEmpty(cpf))
            {
                var usuarioLogado = await _usuarioRepository.GetUsuarioById(Convert.ToInt32(idUsuario));
                var cliente = await _clienteRepository.ObterClientePorCpf(cpf);
                var titular = await _clienteRepository.ObterClientePorCpf(usuarioLogado.Cpf);

                if (cliente != null && titular != null)
                {
                    if (cliente.Cduotitul == titular.Cduop && cliente.Sqtitulmat == titular.Sqmatric)
                    {
                        return View(cliente);
                    }
                }
            }
            return View();
        }

        [Route("partialview-recarga/{cpf}")]
        public async Task<IActionResult> PartialViewRecarga(string cpf)
        {
            var cliente = await _clienteRepository.ObterClientePorCpf(cpf);

            return PartialView("_PartialViewRecarga", cliente);
        }


        [Route("login-ava")]
        public async Task<IActionResult> LoginMoodle(string cpf)
        {
            try
            {

                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Cpf == cpf);
                var cliente = await _clienteRepository.ObterClientePorCpf(cpf);

                if (usuario == null)
                {
                    //var clienteCentral = await _clienteRepository.ObterClientePorCpf(cpf); //CLIENTE NO DB2
                    var novoUsuario = new Usuario
                    {
                        Nome = cliente.Nmcliente,
                        Cpf = cliente.Nucpf,
                        Senha = "dependente menor",
                        ConfirmarSenha = "dependente menor",
                        IsAtivo = true,
                        DataCadastro = DateTime.Now,
                        IdPerfilUsuario = 4,
                        Email = "404",
                        ConfirmaEmail = "404"
                    };
                    await _context.Usuario.AddAsync(novoUsuario);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        code = 2,
                        cpf = cliente.Nucpf
                    });
                }
                if (usuario.Email == "404")
                {
                    return Json(new
                    {
                        code = 2,
                        cpf = usuario.Cpf
                    });
                }


                var token = "d46d0732ccb7f47fa8a09736fa7a65c6";
                //var token = "fddd1e6f9ef1eefa62fcaa60642f9361";
                var moodleUrl = "https://educaonline.sesc-pa.com.br/moodle";
                var functionName = "auth_userkey_request_login_url";
                var nome = cliente != null ? cliente.Nmcliente.Split(new[] { ' ' }, 2) : usuario.Nome.Split(new[] { ' ' }, 2);
                var cpfAluno = cliente != null ? cliente.Nucpf : usuario.Cpf;
                var dados = new[]
                {
                    new KeyValuePair<string, string>("user[firstname]", nome[0]),
                    new KeyValuePair<string, string>("user[lastname]", nome[1]),
                    new KeyValuePair<string, string>("user[email]", usuario.Email),
                    new KeyValuePair<string, string>("user[username]", cpfAluno)                };

                var _client = new HttpClient();
                var formContent = new FormUrlEncodedContent(dados);
                var url = $"{moodleUrl}/webservice/rest/server.php?wstoken={token}&wsfunction={functionName}&moodlewsrestformat=json";
                var result = await _client.PostAsync(url, formContent);
                if (result.IsSuccessStatusCode)
                {
                    var loginUrl = JsonConvert.DeserializeObject<LoginMoodle>(await result.Content.ReadAsStringAsync());
                    if (!string.IsNullOrEmpty(loginUrl.LoginUrl))
                    {
                        return Json(new
                        {
                            code = 1,
                            url = loginUrl.LoginUrl
                        });
                    }
                    return Json(new
                    {
                        code = 0,
                    });

                    //return loginUrl;
                }
                return Json(new
                {
                    code = 0,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new
                {
                    code = 0,
                });
            }
        }


        [Route("redirect-marmita")]
        public async Task<IActionResult> RedirectToMarmitaApp(string nome, string userId /*cpf*/, string email, string credencial)
        {
            /*var tokenService = new TokenService();
            var token = tokenService.GenerateToken(nome, userId, email, credencial);*/

            var token = await _clienteRepository.gerarToken(userId);
            //deve ser alterado para endereço em produção
            if (token != null)
            {
                var marmitaAppUrl = "https://refeicao.sesc-pa.com.br/auth/authenticator?token=" + token;
                return Redirect(marmitaAppUrl);
            }

            return RedirectToAction("Index", "Home");



        }
    }
}
