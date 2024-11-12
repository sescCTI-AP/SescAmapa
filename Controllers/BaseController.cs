using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SiteSesc.Repositories;

namespace SiteSesc.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected int perfil;
        public string idUsuario;
        protected string guid;
        protected string cpf;
        public static UsuarioRepository _usuarioRepository;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            cpf = ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
            perfil = int.Parse(((ClaimsIdentity)User.Identity).FindFirst("Perfil").Value);
            idUsuario = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;
            guid = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;
        }

    }
}
