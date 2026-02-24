using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.Models;
using PagamentoApi.Repositories;

namespace PagamentoApi.Controllers
{
    
    [Route("v1/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        // GET v1/cartao/5
        [HttpGet("{cartao}")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObterSaldo([FromServices] CartaoRepository cartaoRepository, int cartao)
        {
            return await cartaoRepository.ObterSaldo(cartao);
        }

        [HttpGet("cpf/{cpf}")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObterSaldoPorCpf([FromServices] CartaoRepository cartaoRepository,
        [FromServices] ClientelaRepository clientelaRepository, string cpf)
        {
            var cliente = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            if (cliente.NUMCARTAO == null) return 0;
            return await cartaoRepository.ObterSaldo(cliente.NUMCARTAO.Value);
        }

        [HttpGet("movimentacoes/{cartao}")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObterMovimentacoes([FromServices] CartaoRepository cartaoRepository, int cartao)
        {
            return await cartaoRepository.ObterMovimentacaoCartaoCliente(cartao);
        }

        [HttpPost("recarga")]
        [Authorize]
        public async Task<ActionResult> RecarregaCartao([FromServices] CartaoRepository cartaoRepository, [FromServices] CaixaRepository caixaRepository, [FromBody] Recarga recarga)
        {
            //if (!(recarga is Recarga))
            //return BadRequest("Devem ser informados o numero do cartão e o valor da recarga");
            var cacaixa = await caixaRepository.ObterCaixaAberto();
            if (!(cacaixa is CACAIXA))
                return BadRequest("Não existe um caixa aberto");

            var caixadepret = await caixaRepository.ObterCaixaDePret(cacaixa);

            var ultimaMovimentacao = await cartaoRepository.ObterUltimaMovimentacaoCartaoCliente(recarga.NumCartao, cacaixa);

            var result = await cartaoRepository.RecargaCartao(recarga.NumCartao, recarga.Valor, ultimaMovimentacao, caixadepret, cacaixa);

            if (result == "")
                return Ok("Cartão recarregado com sucesso.");
            return BadRequest(result);

        }


        [HttpPost("recarga-avulsa")]
        [Authorize]
        public async Task<ActionResult> RecargaAvulsaCartao([FromServices] CartaoRepository cartaoRepository, [FromServices] CaixaRepository caixaRepository, [FromBody] RecargaAvulsa recarga)
        {
            //if (!(recarga is Recarga))
            //return BadRequest("Devem ser informados o numero do cartão e o valor da recarga");

            var cacaixa = await caixaRepository.ObterCaixaAberto(recarga.IsTef);
            if (!(cacaixa is CACAIXA))
                return BadRequest("Não existe um caixa aberto");

            var caixadepret = await caixaRepository.ObterCaixaDePret(cacaixa);
            var ultimaMovimentacao = await cartaoRepository.ObterUltimaMovimentacaoCartaoCliente(recarga.NumCartao, cacaixa);
            var result = await cartaoRepository.RecargaCartao(recarga.NumCartao, recarga.Valor, ultimaMovimentacao, caixadepret, cacaixa, recarga.CdMoeda, recarga.IsTef);

            if (result == "")
                return Ok("Cartão recarregado com sucesso.");

            return BadRequest(result);

        }
    }
    
}