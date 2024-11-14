using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models;
using SiteSesc.Models.ModelPartialView;
using SiteSesc.Repositories;
using System.Diagnostics;
using SiteSesc.Models.Atividade;
using SiteSesc.Services;
using SiteSesc.Data;

namespace SiteSesc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SiteSescContext _context;
        private readonly AreaRepository _areaRepository;
        private readonly UnidadeRepository _unidadeRepository;
        private readonly AtividadeOnLineReposotory _atividadeRepository;
        private readonly DefaultRepository _defaultRepository;
        private readonly SafeExecutor _safeExecutor;
        private UsuarioRepository _usuarioRepository;
        public readonly HostConfiguration hostConfiguration;
        private readonly IConfiguration configuration;
        private IWebHostEnvironment _server;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger,
            IConfiguration configuration,
            SiteSescContext context,
            AreaRepository areaRepository,
            UnidadeRepository unidadeRepository,
            UsuarioRepository usuarioRepository,
            AtividadeOnLineReposotory atividadeOnLineReposotory,
            SafeExecutor safeExecutor,
            IWebHostEnvironment server,
            EmailService emailService,
            DefaultRepository defaultRepository)
        {
            _logger = logger;
            this.configuration = configuration;
            _context = context;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _areaRepository = areaRepository;
            _unidadeRepository = unidadeRepository;
            _usuarioRepository = usuarioRepository;
            _atividadeRepository = atividadeOnLineReposotory;
            _defaultRepository = defaultRepository;
            _safeExecutor = safeExecutor;
            _server = server;
            _emailService = emailService;

        }

        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Atividades", "Atividade");
        }


        [HttpGet("get-atividades/{id?}/{cduop?}")]
        public async Task<IActionResult> GetAtividades(int? id, int? cduop = null)
        {
            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var listCursos = new List<CursoItem>();

                var atividades = await _atividadeRepository.GetAtivas(id, cduop, 8);
                if (atividades is List<AtividadeOnLine>)
                {
                    var listaAtividades = (List<AtividadeOnLine>)atividades;
                    foreach (var atividade in listaAtividades)
                    {
                        var atividadeApi = await _atividadeRepository.ObterAtividade(atividade.turma);
                        if (atividadeApi != null)
                        {
                            var formasPgto = await _atividadeRepository.ObterFormasPgtoCdelement(atividade.cdelement);
                            var horarios = await _atividadeRepository.ObterHorariosCdelement(atividade.cdelement);
                            var valorAtividade = await _atividadeRepository.ObterValores(atividade.turma, formasPgto);
                            var valor = valorAtividade.Count() > 0 ? valorAtividade.Min(va => va.vlparcela).ToString() : null;
                            listCursos.Add(new CursoItem((int)atividade.SubArea.IdArea, atividade.cdelement, atividade.Arquivo.CaminhoVirtualFormatado(), atividade.NomeExibicao, atividade.SubArea.Area.Nome, atividade.UnidadeOperacional.Nome, valor, atividade.Descricao, null));
                        }
                    }
                }
                return Ok(listCursos);
            });
        }


        [HttpGet("get-atividades-uop/{id?}")]
        public async Task<IActionResult> GetAtividadesUop(int? id)
        {
            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var listCursos = new List<CursoItem>();

                var atividades = await _atividadeRepository.GetAtivasUop(id);
                if (atividades is List<AtividadeOnLine>)
                {
                    var listaAtividades = (List<AtividadeOnLine>)atividades;
                    foreach (var atividade in listaAtividades)
                    {
                        var atividadeApi = await _atividadeRepository.ObterAtividade(atividade.turma);
                        var formasPgto = await _atividadeRepository.ObterFormasPgtoCdelement(atividade.cdelement);
                        var horarios = await _atividadeRepository.ObterHorariosCdelement(atividade.cdelement);
                        var valorAtividade = await _atividadeRepository.ObterValores(atividade.turma, formasPgto);
                        var valor = valorAtividade != null ? valorAtividade.Min(va => va.vlparcela).ToString() : null;
                        listCursos.Add(new CursoItem((int)atividade.SubArea.IdArea, atividade.cdelement, atividade.Arquivo.CaminhoVirtualFormatado(), atividade.NomeExibicao, atividade.SubArea.Area.Nome, atividade.UnidadeOperacional.Nome, valor, atividade.Descricao, null));
                    }
                }
                return Ok(listCursos);
            });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("Home/Error")]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue && statusCode.Value == 500)
            {
                return View("error");
            }
            if (statusCode.HasValue && statusCode.Value == 404)
            {
                return View("notfound");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}