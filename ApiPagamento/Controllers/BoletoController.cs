using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.Clients;
using PagamentoApi.Models.BB;
using PagamentoApi.Repositories;

namespace PagamentoApi.Controllers
{
    
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class BoletoController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<dynamic>> ObtemToken([FromServices] BBClient bbClient)
        {
            var autorizacao = await bbClient.ObterToken();
            return autorizacao;
        }

        [HttpGet("aberto")]
        public async Task<ActionResult<dynamic>> ObtemBoletosEmAberto([FromServices] BBClient bbClient)
        {
            var boletos = await bbClient.ListarBoletos('A');
            return boletos;
        }

        [HttpGet("baixados")]
        public async Task<ActionResult<dynamic>> ObtemBoletosBaixados([FromServices] BBClient bbClient)
        {
            var boletos = await bbClient.ListarBoletos('B', 7);
            return boletos;
        }

        [HttpGet("liquidados")]
        public async Task<ActionResult<dynamic>> ObtemBoletosLiquidados([FromServices] BBClient bbClient)
        {
            var dataAtual = DateTime.Now;
            var boletos = await bbClient.ListarBoletos('B', 6, dataAtual, dataAtual);
            return boletos;
        }

        [HttpGet("agendados")]
        public async Task<ActionResult<dynamic>> ObtemBoletosAgendados([FromServices] BBClient bbClient)
        {
            var dataAtual = DateTime.Now;
            var boletos = await bbClient.ListarBoletos('A', 0, dataAtual, dataAtual);
            return boletos;
        }

        [HttpGet("validar-boletos-pagos")]
        public async void ValidarBoletosPagos([FromServices] BBClient bbClient, [FromServices] BoletoRepository boletoRepository)
        {
            var dataAtual = DateTime.Now;
            var boletosBaixados = await bbClient.ListarBoletos('B', 7, dataAtual, dataAtual);
            if (boletosBaixados is BoletoResponse)
            {
                var retornoBaixados = await boletoRepository.BaixarBoletos(boletosBaixados.boletos, bbClient);
                if (retornoBaixados)
                    Console.WriteLine($"{DateTime.Now:hh:mm:ss} Boletos Baixados.");
            }
            //Boletos pagos
            var retorno = await boletoRepository.LiquidarBoletos(bbClient);

        }

        [HttpGet("detalhes/{id}")]
        public async Task<ActionResult<dynamic>> ObtemBoletosBaixados([FromServices] BBClient bbClient, string id)
        {
            var boletos = await bbClient.ObterDetalhesBoletos(id);
            return boletos;
        }  

        [HttpGet("baixar/{id}")]
        public async Task<ActionResult<dynamic>> BaixarBoleto([FromServices] BBClient bbClient, string id)
        {
            var boletos = await bbClient.BaixarBoleto(id);
            return boletos;
        }

        [HttpPost]
        public async Task<ActionResult<dynamic>> GerarBoleto([FromServices] BBClient bbClient, [FromBody] BoletoRequest boleto)
        {
            
            var boletoRetorno = await bbClient.GerarBoleto(boleto);
            return boletoRetorno;
        }

        [HttpGet("MovimentoDoDia/{data}")] 
        public async Task<ActionResult<dynamic>> ObtemBoletosMovimentoDoDia([FromServices] BBClient bbClient, DateTime data)
        {
            var dataAtual = data;            
            var boletos = await bbClient.ListarBoletos('B', 0, dataAtual, dataAtual);
            return boletos;
        }
    }
    
}