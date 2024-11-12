using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models.ModelPartialView;

namespace SiteSesc.Controllers
{
    public class ConteudoEstaticoController : Controller
    {
        [Route("credencial")]
        public IActionResult Credencial()
        {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }

        [Route("estrutura-organizacional")]
        public IActionResult EstruturaOrganizacional() {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }

        [Route("fale-conosco")]
        public IActionResult FaleConosco()
        {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }

        [Route("imprensa")]
        public IActionResult Imprensa()
        {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }

        [Route("sobre-sesc")]
        public IActionResult SobreSesc()
        {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }

        [Route("Circuito")]

        public IActionResult Circuito()
        {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }



        [Route("TermosDeUso")]

        public IActionResult TermosDeUso()
        { 
            return View();
        }


        [Route("PoliticaPrivacidade")]
        public IActionResult PoliticaPrivacidade()
        {
            var listCategorias = new List<CategoriaCard> {
                new CategoriaCard("../images/static/HomeIndex/category/1.webp", "Educação", 26),
                new CategoriaCard("../images/static/HomeIndex/category/2.webp", "Esporte/Lazer", 42),
                new CategoriaCard("../images/static/HomeIndex/category/3.webp", "Cultura", 25),
                new CategoriaCard("../images/static/HomeIndex/category/4.webp", "Saúde", 18),
                new CategoriaCard("../images/static/HomeIndex/category/5.webp", "Assistência", 18)
            };
            ViewBag.ListCategorias = listCategorias;
            return View();
        }
    }
}
