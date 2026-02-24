using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.Models;
using PagamentoApi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PagamentoApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class VendasController : ControllerBase
    {
        [HttpGet("itens/{inicio}/{fim}")]
        [HttpGet("itens/{inicio}/{fim}/{cduop}")]
        [Authorize(Roles = "app")]
        public async Task<ActionResult<List<dynamic>>> ObterVendasPorItem([FromServices] VendasRepository vendasRepository, DateTime inicio, DateTime fim, int? cduop = null)
        {

            var vendas = await vendasRepository.ObterVendasPorItens(inicio, fim, cduop);

            if (vendas is List<ItemVenda>)
                return Ok(vendas);

            return BadRequest("Erro ao recuperar atividades, verifique o codigo da unidade.");
        }
    }
}
