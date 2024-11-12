using Microsoft.AspNetCore.Mvc;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ModelPartialView;
using SiteSesc.Repositories;

namespace SiteSesc.Controllers
{
    [Route("unidades")]
    public class UnidadeController : Controller
    {
        private readonly UnidadeRepository _unidadeRepository;

        public UnidadeController(UnidadeRepository unidadeRepository)
        {
            _unidadeRepository = unidadeRepository;
        }

       
        public async Task<IActionResult> Index()
        {
            var unidades = await _unidadeRepository.GetUOAtiva();
            var count    = unidades.Count();
            ViewBag.count = count;
            return View(unidades);
        }

        [Route("{nome}")]
        public async Task<IActionResult> Details(string nome)
        {
            var unidade = await _unidadeRepository.GetUOName(nome);

            return View(unidade);
        }



    }
}