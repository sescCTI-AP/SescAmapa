using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models.Enums;
using SiteSesc.Models;
using SiteSesc.Repositories;
using System.Security.Claims;

namespace SiteSesc.Services
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionFilter : ActionFilterAttribute
    {
        private readonly int _permissaoNecessaria;
        public List<int>? acoesUsuario;
        private int _idModulo = 0;

        public PermissionFilter(int permissaoNecessaria, int idModulo)
        {
            _permissaoNecessaria = permissaoNecessaria;
            _idModulo = idModulo;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var idUsuario = context.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "Id")?.Value;
            var usuarioRepository = context.HttpContext.RequestServices.GetService(typeof(UsuarioRepository)) as UsuarioRepository;

            if (_idModulo > 0)
            {
                acoesUsuario = usuarioRepository.GetAcoesUsuarioModulo(Convert.ToInt32(idUsuario), _idModulo).Result;
            }

            if (acoesUsuario == null || !acoesUsuario.Contains(_permissaoNecessaria))
            {
                context.Result = new ViewResult()
                {
                    ViewName = "AcessoNegado"
                };
            }

            base.OnActionExecuting(context);
        }
    }
}
