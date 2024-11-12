using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SiteSesc.Models;
using SiteSesc.Repositories;

namespace SiteSesc.Areas.Cliente.Controllers
{
    [Authorize]
    [Area("Cliente")]
    public class BaseClienteController : Controller
    {
        protected int perfil;
        public string idUsuario;
        protected string guid;
        protected string cpf;
        public List<int>? acoesUsuario;
        public static UsuarioRepository _usuarioRepository;
        public static int idModulo = 0;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            perfil = Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst("Perfil").Value);
            idUsuario = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;
            guid = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;
            cpf = ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
            if (idModulo > 0) {
                acoesUsuario = _usuarioRepository.GetAcoesUsuarioModulo(Convert.ToInt32(idUsuario), idModulo).Result;
            }

            ViewBag.Acoes = acoesUsuario;
        }

    }
}
