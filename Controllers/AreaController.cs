using Microsoft.AspNetCore.Mvc;
using SiteSesc.Data;
using SiteSesc.Models.ModelPartialView;
using SiteSesc.Repositories;

namespace SiteSesc.Controllers
{
    [Route("area")]
    public class AreaController : Controller
    {
        private readonly AreaRepository _areaRepository;

        public AreaController(AreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        [Route("{nome3}")]
        public async Task<IActionResult> Index(string nome3)
        {
            var area = await _areaRepository.GetAreaPorNome(nome3);
            ViewBag.SubAreas = await _areaRepository.GetSubAreaPorArea(area.Id);

            //var listNoticias = new List<NoticiaCard>();

            //var noticias = await _noticiaRepository.ObtemListaNoticiasPorArea(area.Id, 6);
            //if (noticias != null)
            //{
            //    if (noticias.Any())
            //    {
            //        foreach (var noticia in noticias)
            //            listNoticias.Add(new NoticiaCard(noticia.Id.ToString(), noticia.Arquivo.CaminhoVirtualFormatado(), noticia.Area.Nome, noticia.Cidade.Nome, noticia.Slug, noticia.TituloCurto, area.NameRoute));
            //    }
            //}
            //ViewBag.ListNoticias = listNoticias;
            return View(area);
        }
    }
}