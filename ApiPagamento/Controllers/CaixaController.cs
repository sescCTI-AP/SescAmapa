using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.Models;
using PagamentoApi.Repositories;

namespace PagamentoApi.Controllers
{
    
    [Route("v1/[controller]")]
    [ApiController]
    public class CaixaController : ControllerBase
    {
        public CaixaController() { }

        // GET v1/caixa
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObtemCaixa([FromServices] CaixaRepository caixaRepository)
        {
            var caixa = await caixaRepository.ObterCaixaAberto();
            if (caixa is CACAIXA)
                return caixa;
            else
                return BadRequest("Não existe um caixa aberto");
        }

        [HttpGet("lista")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObtemListaCaixas([FromServices] CaixaRepository caixaRepository)
        {
            var caixas = await caixaRepository.ObterListaCaixas("2020");
            return caixas;
        }

        [HttpGet("extrato/{caixa}")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObtemExtratoCaixa([FromServices] CaixaRepository caixaRepository, [FromServices] CobrancaRepository cobrancaRepository, int caixa)
        {
            var cacaixa = await caixaRepository.ObterCaixa(caixa);
            var extratoCobrancas = await cobrancaRepository.ObterCobrancasPagasPorCaixa(cacaixa);
            var extratoRecargas = await caixaRepository.ObterLancamentosPdv(cacaixa);

            return Ok(
                new
                {
                    extratoCobrancas,
                    extratoRecargas
                }
            );
        }

        // GET v1/caixa/caixadepret
        [HttpGet("caixadepret")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObtemCaixaDePret([FromServices] CaixaRepository caixaRepository)
        {
            var cacaixa = await caixaRepository.ObterCaixaAberto();
            if (!(cacaixa is CACAIXA))
                return BadRequest("Não existe um caixa aberto");

            var caixadepret = await caixaRepository.ObterCaixaDePret(cacaixa);
            if (caixadepret is CXDEPRETPDV)
                return caixadepret;
            return BadRequest("Não existe um caixa de pret");
        }

        // GET v1/caixa/abrir
        [HttpPost("abrir")]
        [Authorize]
        public async Task<ActionResult<dynamic>> AbrirCaixa([FromServices] CaixaRepository caixaRepository)
        {

            var result = await caixaRepository.AbrirCaixa();
            if (result is CACAIXA)
                return Ok(result);
            return BadRequest(result);
        }

        // GET v1/caixa/5
        [HttpPost("fechar")]
        [Authorize]
        public async Task<ActionResult<dynamic>> FecharCaixa([FromServices] CaixaRepository caixaRepository)
        {
            var result = await caixaRepository.FecharCaixa();
            if (result.Codigo == 0)
                return Ok(result);
            return BadRequest(result);
        }

        // GET v1/caixa/retirada
        [HttpPost("retirada")]
        [Authorize]
        public async Task<ActionResult<dynamic>> EfetuarRetiradaDoCaixa([FromServices] CaixaRepository caixaRepository)
        {
            var result = await caixaRepository.EfetuarRetiradaDoCaixa();
            if (result is CAIXALANCA)
                return Ok(result);
            return BadRequest(result);
        }
    }
    
}