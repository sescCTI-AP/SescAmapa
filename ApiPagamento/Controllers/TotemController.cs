using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.Models.Tef;
using PagamentoApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PagamentoApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class TotemController : Controller
    {

        [HttpGet("obtem-totem/{ip}")]
        [Authorize]
        public async Task<ConfigTotem> ObtemTotem([FromServices] TotemRepository totemRepository, string ip)
        {
            var configTotem = await totemRepository.ObtemTotem(ip);
            return configTotem;
        }
        [HttpGet("obtem-totem-codigo/{codigo}")]
        [Authorize]
        public async Task<ConfigTotem> ObtemTotemCodigo([FromServices] TotemRepository totemRepository, string codigo)
        {
            var configTotem = await totemRepository.ObtemTotemCodigo(codigo);
            return configTotem;
        }

        [HttpGet("obtem-cupom")]
        [Authorize]
        public async Task<int> ObtemCupom([FromServices] TotemRepository totemRepository)
        {
            var cupom = await totemRepository.ObtemCupom();
            return cupom;
        }

        [HttpGet("obtem-transacoes/{cpf}")]
        [Authorize]
        public async Task<List<TransacaoTotem>> ObtemTransacoesPorCliente([FromServices] TotemRepository totemRepository, string cpf)
        {
            var configTotem = await totemRepository.ObtemTransacoesPorCliente(cpf);
            return configTotem;
        }


    }
}
