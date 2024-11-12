using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiteSesc.Data;
using SiteSesc.Helpers;
using SiteSesc.Models;
using SiteSesc.Models.Avaliacao;
using SiteSesc.Repositories;
using SiteSesc.Services;

namespace SiteSesc.Areas.Cliente.Controllers
{
    [Authorize]
    [Area("Cliente")]
    [Route("cliente/atividade")]
    public class AtividadeController : BaseClienteController
    {
        private readonly SiteSescContext _context;
        private IWebHostEnvironment _env;
        private readonly IConfiguration configuration;
        public readonly HostConfiguration hostConfiguration;
        private IMemoryCache cache;
        private Usuario usuario;
        private bool devMode;
        private readonly ClienteRepository _clienteRepository;
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly SafeExecutor _safeExecutor;
        private readonly AtividadeOnLineReposotory _atividadeOnLineRepository;
        private IWebHostEnvironment _server;
        private readonly EmailService _emailService;



        public AtividadeController(SiteSescContext context,
            IWebHostEnvironment env,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IWebHostEnvironment server,
            UsuarioRepository usuarioRepository,
            AtividadeOnLineReposotory atividadeOnLineRepository,
            ClienteRepository clienteRepository,
            SafeExecutor safeExecutor,
                        EmailService emailService,
            CobrancaRepository cobrancaRepository)
        {
            this.configuration = configuration;
            cache = memoryCache;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _context = context;
            _env = env;
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _safeExecutor = safeExecutor;
            _cobrancaRepository = cobrancaRepository;
            _atividadeOnLineRepository = atividadeOnLineRepository;
            _server = server;
            _emailService = emailService;

        }

        [Route("detalhes/{cdelement}/{cpfCliente}")]
        public async Task<IActionResult> DetailsAtividade(string cdelement, string cpfCliente)
        {
            if (string.IsNullOrEmpty(cdelement) || string.IsNullOrEmpty(cpfCliente))
                return NotFound();

            cpf = cpfCliente;

            var cliente = await _usuarioRepository.ObterUsuarioPorCpf(cpf);

            ViewBag.Avaliacoes = await _atividadeOnLineRepository.ObtemAvaliacoes(cdelement, cliente.Id);
            ViewBag.Cliente = cliente;

            var atividades = await _atividadeOnLineRepository.GetMatriculas(cpf);
            if (atividades != null)
            {
                if (atividades.Any())
                {
                    var atividade = atividades.FirstOrDefault(m => m.cdelement == cdelement);
                    return View(atividade);
                }
            }

            return NotFound();
        }

        [Route("avalicao-atividade")]
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> AvaliacaoAtividade([FromForm] AvaliacaoAtividadeCliente avaliacaoAtividadeCliente)
        {
            //var cpf = ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
            if (cpf != null)
            {
                var cliente = await _usuarioRepository.ObterUsuarioPorCpf(cpf);
                var novaAvaliacao = await _atividadeOnLineRepository.AvaliacaoAtividadeCliente(avaliacaoAtividadeCliente, cliente.Id);
                return RedirectToAction("DetailsAtividade", new { cdelement = avaliacaoAtividadeCliente.cdelement });
            }
            return RedirectToAction("Login", "Home", new { area = "" });

        }

    }
}
