using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ModelPartialView;
using SiteSesc.Repositories;
using SiteSesc.Services;
using System.Security.Claims;

namespace SiteSesc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("admin")]
    public class HomeController : BaseController
    {
        private IWebHostEnvironment _env;
        private readonly IConfiguration configuration;
        public readonly HostConfiguration hostConfiguration;
        private IMemoryCache cache;
        private Usuario usuario;
        private bool devMode;
        private DefaultRepository _defaultRepository;
        private readonly SafeExecutor _safeExecutor;


        public HomeController(IWebHostEnvironment env,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            UsuarioRepository usuarioRepository,
            SafeExecutor safeExecutor,
            DefaultRepository defaultRepository)
        {
            this.configuration = configuration;
            cache = memoryCache;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _env = env;
            _usuarioRepository = usuarioRepository;
            _defaultRepository = defaultRepository;
            _safeExecutor = safeExecutor;

        }

        [Route("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var usuario = await _usuarioRepository.GetUsuario(new Guid(guid));


            //#region Codigos para mockup
            //var listDependentes = new List<ClienteDetails>
            //{
            //    new ClienteDetails("Simone", "Esposa", "../images/pressets/simone.jpeg", "simone", "Simone"),
            //    new ClienteDetails("Rafael", "Filho", "../images/pressets/rafa.jpeg", "rafa", "Rafael"),
            //    new ClienteDetails("Gabriela", "Filha", "../images/pressets/gabi.jpeg", "gabi", "Gabriela"),
            //    new ClienteDetails("Mercê", "Mãe", "../images/pressets/merce.jpeg", "merce", "Mercê"),
            //    new ClienteDetails("Raimundo", "Pai", "../images/pressets/raimundo.jpeg", "raimundo", "Raimundo")
            //};

            //var listCobrancas = new List<CobrancaDetails> {
            //    new CobrancaDetails("bx bx-dumbbell", "MUSCULAÇÃO 2X (3ª E 5ª)- 6H/10H - 12H/22H - SÁB 9H/13H - CAP 2023", "Vencimento: 15/08/2023", "Valor: R$ 35,00", "Pague hoje: R$ 34,18"),
            //    new CobrancaDetails("bx bx-swim", "NATAÇÃO (+ de 9 anos) - 2ª, 4ª e 6ª - 17h15 às 18h", "Vencimento: 15/08/2023", "Valor: R$ 35,00", "Pague hoje: R$ 34,18")
            //};

            //var listInscricao = new List<InscricaoDetails> {
            //    new InscricaoDetails("MUSCULAÇÃO 2X (3ª E 5ª)- 6H/10H - 12H/22H - SÁB 9H/13H - CAP 2023", new List<string> { "#", "#", "#" }, new List<string> { "bx bx-search-alt", "bx bx-dollar-circle", "bx bx-file-blank" }, new List<string> { "Detalhes", "Mensalidades", "Ver termo de adesão" }),
            //    new InscricaoDetails("AVALIAÇÃO FÍSICA", new List<string> { "#", "#", "#" }, new List<string> { "bx bx-search-alt", "bx bx-dollar-circle", "bx bx-file-blank" }, new List<string> { "Detalhes", "Mensalidades", "Ver termo de adesão" })
            //};
            //#endregion
            //ViewBag.Dependentes = listDependentes;
            //ViewBag.Cobrancas = listCobrancas;
            //ViewBag.Inscricoes = listInscricao;
            return View(usuario);
        }

        [Route("tela-inicial")]
        public async Task<IActionResult> DefinirTelaInicial(string rota)
        {
            var salva = await _usuarioRepository.DefinirTelaInicial(Convert.ToInt32(idUsuario), rota);
            if (salva)
            {
                return Ok(1);
            }
            return BadRequest();
        }


        [HttpGet("get-dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            return await _safeExecutor.ExecuteSafe(async () =>
            {
                var usuario = await _usuarioRepository.GetUsuario(new Guid(guid));
                var actionDashboard = await _defaultRepository.GetDashboard(usuario.Id);
                return Ok(actionDashboard);
            });
        }

    }
}
