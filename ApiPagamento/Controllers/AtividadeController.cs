using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Site;
using PagamentoApi.Repositories;
using PagamentoApi.Settings;
//using PagamentoApi.Models;

namespace PagamentoApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AtividadeController : ControllerBase
    {
        public readonly IConfiguration configuration;
        private readonly CaixaSettings caixaConfiguration;

        public AtividadeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            caixaConfiguration = configuration.GetSection("CaixaSettings").Get<CaixaSettings>();
        }



        [HttpGet("app/{cpf}")]
        [Authorize(Roles = "usuario")]
        public async Task<ActionResult<List<dynamic>>> ObterAtividadesDoClienteLogado([FromServices] ClientelaRepository clientelaRepository,
         [FromServices] AtividadesRepository atividadesRepository, string cpf)
        {
            var cliente = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            var atividades = await atividadesRepository.ObterAtividadesPorCliente(cliente);

            var atividadesApp = new List<AtividadeApp>();

            foreach (var atividade in atividades)
            {
                atividadesApp.Add(new AtividadeApp
                {
                    CDPROGRAMA = atividade.CDPROGRAMA,
                    CDCONFIG = atividade.CDCONFIG,
                    SQOCORRENC = atividade.SQOCORRENC,
                    CDFONTEINF = atividade.CDFONTEINF,
                    Atividade = atividade.PROGOCORR.DSUSUARIO.Trim(),
                    Inscricao = atividade.DTINSCRI.Value
                });
            }
            return Ok(atividadesApp);
        }

        [HttpGet("unidade/{uop}")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterAtividadesPorUnidade([FromServices] AtividadesRepository atividadesRepository, int uop)
        {
            var atividades = await atividadesRepository.ObterAtividades(uop);
            if (atividades is List<PROGRAMAS>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar atividades, verifique o codigo da unidade.");
        }


        [HttpGet("unidade/modalidade/{uop}")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterAtividadesComModalidadePorUnidade([FromServices] AtividadesRepository atividadesRepository, int uop)
        {
            var atividades = await atividadesRepository.ObterAtividadesPorUnidadeEModalidade(uop);
            if (atividades is List<PROGRAMAS>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar atividades, verifique o codigo da unidade.");
        }

        [HttpGet("termo/{cdprograma}/{ano}")]
        public async Task<ActionResult<List<dynamic>>> ObterTermoDeAdesaoPorAno([FromServices] AtividadesRepository atividadesRepository, int cdprograma, int ano)
        {
            var termo = await atividadesRepository.ObterTermoDeAdesaoPorAno(cdprograma, ano);
            return Ok(new { termo = termo });
            //return BadRequest("Erro ao recuperar atividades, verifique o codigo da unidade.");
        }

        [HttpGet("termo/{cdprograma}")]
        public async Task<ActionResult<List<dynamic>>> ObterTermoDeAdesao([FromServices] AtividadesRepository atividadesRepository, int cdprograma)
        {
            var termo = await atividadesRepository.ObterTermoDeAdesao(cdprograma);
            return Ok(new { termo = termo });
            //return BadRequest("Erro ao recuperar atividades, verifique o codigo da unidade.");
        }

        [HttpPost("valores")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterValoresTurma([FromServices] AtividadesRepository atividadesRepository, [FromBody] PROGOCORR turma)
        {
            var valores = await atividadesRepository.ObtemValoresTurma(turma);
            if (valores is List<VALORPARC>)
                return Ok(valores);
            return BadRequest("Erro ao recuperar valores da turma, verifique os dados informados.");
        }

        [HttpPost("turma")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterDadosTurma([FromServices] AtividadesRepository atividadesRepository, [FromBody] PROGOCORR turma)
        {
            var turmaDb = await atividadesRepository.ObtemDadosTurma(turma);
            if (turmaDb is PROGOCORR)
                return Ok(turmaDb);
            return BadRequest("Erro ao recuperar dados da turma, verifique os dados informados.");
        }

        [HttpGet("turmas/{uop}")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterTurmasPorUnidade([FromServices] AtividadesRepository atividadesRepository, int uop)
        {
            var atividades = await atividadesRepository.ObterTurmasPorUnidade(uop);
            if (atividades is List<PROGOCORR>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar turmas, verifique o codigo da unidade.");
        }

        [HttpGet("turmas/inscritos-por-data/{uop}/{data}")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterTurmasComPorCategoriaEData([FromServices] AtividadesRepository atividadesRepository, int uop, DateTime data)
        {
            var atividades = await atividadesRepository.ObterTurmasPorUnidadeEData(uop, data);
            if (atividades is List<ModalidadeCategoria>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar turmas, verifique o codigo da unidade.");
        }

        [HttpGet("turmas/inscritos/{uop}/{ano}/{mes}")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterTurmasComPorCategoriaEData([FromServices] AtividadesRepository atividadesRepository, int uop, int? ano = null, int? mes = null)
        {
            var atividades = await atividadesRepository.ObterTurmasPorUnidadeEData(uop, ano, mes);
            if (atividades is List<ModalidadeCategoria>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar turmas, verifique o codigo da unidade.");
        }

        [HttpGet("turmas/inscritos/{uop}/{ano}")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterTurmasComInscritosPorUnidade([FromServices] AtividadesRepository atividadesRepository, int uop, int? ano = null)
        {
            var atividades = await atividadesRepository.ObterTurmasComInscritosPorUnidade(uop, ano);
            if (atividades is List<PROGOCORR>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar turmas, verifique o codigo da unidade.");
        }

        [HttpGet("{cdprograma}/{cdconfig}/{sqocorrenc}")]
        [Authorize]
        public async Task<ActionResult<AtividadeOnLine>> ObterAtividadesOnline([FromServices] AtividadesRepository atividadesRepository, int cdprograma, int cdconfig, int sqocorrenc)
        {
            var atividades = await atividadesRepository.ObterAtividadeSite(cdprograma, cdconfig, sqocorrenc);
            if (atividades is AtividadeOnLine)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar turmas, verifique o codigo da unidade.");
        }

        [HttpGet("modalidades")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterModalidades([FromServices] AtividadesRepository atividadesRepository)
        {
            var modalidades = await atividadesRepository.ObterModalidades();
            if (modalidades is List<MODALIDADE>)
                return Ok(modalidades);
            return BadRequest("Erro ao recuperar modalidades.");
        }

        [HttpPost("inscrever")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> InscreverUsuario([FromServices] AtividadesRepository atividadesRepository, [FromServices] ClientelaRepository clientelaRepository, [FromBody] INSCRICAO inscricao)
        {

            var cliente = await clientelaRepository.ObterClientePorMatricula(inscricao.CDUOP, inscricao.SQMATRIC);
            inscricao.CDCATEGORI = cliente.CDCATEGORI;
            inscricao.CLIENTELA = cliente;

            var result = await atividadesRepository.InscreveUsuario(inscricao);
            return Ok(result);
        }

        [HttpPost("mensalidades")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> Mensalidades([FromServices] AtividadesRepository atividadesRepository, [FromServices] ClientelaRepository clientelaRepository, [FromBody] INSCRICAO inscricao)
        {
            var cliente = await clientelaRepository.ObterClientePorMatricula(inscricao.CDUOP, inscricao.SQMATRIC);
            var turma = await atividadesRepository.ObterDadosTurma(inscricao.CDPROGRAMA, inscricao.CDCONFIG, inscricao.SQOCORRENC);

            inscricao.CDCATEGORI = cliente.CDCATEGORI;
            inscricao.CLIENTELA = cliente;
            inscricao.CDPERFIL = await atividadesRepository.ObterPerfilCliente(inscricao);
            inscricao.DTFIMOCORR = turma.DTFIMOCORR != null ? turma.DTFIMOCORR.Value : DateTime.Now;
            inscricao.PROGOCORR = turma;

            var cobrancas = await atividadesRepository.GerarCobranca(inscricao, turma.CDUOPCAD);
            return Ok(cobrancas);
        }


        [HttpPost("cancelar-nova")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> CancelarAtividadeNova([FromServices] AtividadesRepository atividadesRepository, [FromServices] ClientelaRepository clientelaRepository, [FromBody] INSCRICAO inscricao)
        {
            var result = await atividadesRepository.CancelaInscricao(inscricao, true);
            return Ok(result);
        }
        [HttpPost("cancelar")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> CancelarAtividade([FromServices] AtividadesRepository atividadesRepository, [FromServices] ClientelaRepository clientelaRepository, [FromBody] INSCRICAO inscricao)
        {
            var result = await atividadesRepository.CancelaInscricao(inscricao);
            return Ok(result);
        }

        [HttpPost("formas-pagamento")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> FormasPgto([FromServices] AtividadesRepository atividadesRepository, [FromBody] INSCRICAO inscricao)
        {
            var result = await atividadesRepository.ObterFormatoPgto(inscricao.CDPROGRAMA, inscricao.CDCONFIG);
            return Ok(result);
        }
        // GET api/atividade
        [HttpGet("{cpf}")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterAtividadesPorCpf([FromServices] ClientelaRepository clientelaRepository, [FromServices] AtividadesRepository atividadesRepository, string cpf)
        {
            var cliente = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            if(cliente == null)
                return Ok(null);

            var atividades = await atividadesRepository.ObterAtividadesPorCliente(cliente);
            if(atividades == null)
                return Ok(null);

            return Ok(atividades);
        }

        // GET api/atividade
        [HttpGet("inativas/{cpf}")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterAtividadesInativasPorCpf([FromServices] ClientelaRepository clientelaRepository, [FromServices] AtividadesRepository atividadesRepository, string cpf)
        {
            var cliente = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            if (cliente is null)
            {
                return BadRequest("Cliente n�o encontrado");
            }
            var atividades = await atividadesRepository.ObterAtividadesInativasPorCliente(cliente);
            return Ok(atividades);
        }

        [HttpPost("usuarios")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterInscritos([FromServices] AtividadesRepository atividadesRepository, [FromBody] Turma turma)
        {
            var inscricoes = await atividadesRepository.ObtemUsuariosTurma(turma);
            return Ok(inscricoes);
        }

        [HttpPost("cancelamentos")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterCancelamentos([FromServices] AtividadesRepository atividadesRepository, [FromBody] Turma turma)
        {
            var cancelamentos = await atividadesRepository.ObtemCancelamentosTurma(turma);
            return Ok(cancelamentos);
        }

        [HttpGet("turmas-select/{uop}")]
        [Authorize]
        public async Task<ActionResult<List<dynamic>>> ObterTurmasSelect([FromServices] AtividadesRepository atividadesRepository, int uop)
        {
            var atividades = await atividadesRepository.ObterTurmasSelect(uop);
            if (atividades is List<PROGOCORR>)
                return Ok(atividades);
            return BadRequest("Erro ao recuperar turmas, verifique o codigo da unidade.");
        }

        [HttpGet("formas-pagamento-cdelement/{cdelement}")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> FormasPgto([FromServices] AtividadesRepository atividadesRepository, string cdelement)
        {
            var cdprograma = Convert.ToInt32(cdelement.Substring(0, 8));
            var cdconfig = Convert.ToInt32(cdelement.Substring(8, 8));
            var result = await atividadesRepository.ObterFormatoPgto(cdprograma, cdconfig);
            return Ok(result);
        }
    }
}