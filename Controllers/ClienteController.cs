using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models;
using SiteSesc.Models.ModelPartialView;
using System.Diagnostics;

namespace SiteSesc.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ClienteController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}