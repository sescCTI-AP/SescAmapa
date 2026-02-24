using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models.Site;
using PagamentoApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PagamentoApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class NoticiaController : Controller
    {
        public readonly IConfiguration configuration;

        public NoticiaController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("{qtd}")]
        public async Task<List<Noticia>> ObterCardapio([FromServices] NoticiaRepository noticiaRepository, int qtd)
        {
            return await noticiaRepository.ObterNoticias(qtd);
        }
    }
}
