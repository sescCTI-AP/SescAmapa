using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.Enums;
using SiteSesc.Repositories;
using System.Security.Claims;

namespace SiteSesc.Controllers
{
    public class SideMenuViewComponent : ViewComponent
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly SiteSescContext _context;
        ActionExecutingContext context;

        public SideMenuViewComponent(UsuarioRepository usuarioRepository, SiteSescContext context)
        {
            _usuarioRepository = usuarioRepository;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string area)
        {
            var idUsuario = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;
            var idPerfil = Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst("Perfil").Value);

            if (area.Equals("cliente"))
            {
                ViewBag.Botoes = await _context.BotaoSideMenu.Where(b => !b.IsAdmin).OrderBy(a => a.Sequencia).ToListAsync();
            }
            else
            {
                //var botoesPerfil = await _context.BotaoPerfil.Where(b => b.IdPerfilUsuario == idPerfil).Select(a => a.IdBotaoSideMenu).ToListAsync();
                //ViewBag.Botoes = await _context.BotaoSideMenu.Where(b => botoesPerfil.Contains(b.Id) && b.IsAdmin).ToListAsync();
                if (idPerfil != (int)PerfilUsuarioEnum.Administrador && idPerfil != (int)PerfilUsuarioEnum.SysAdmin && idPerfil != (int)PerfilUsuarioEnum.Coordernador)
                {
                    List<int> modulosUsuario = await _usuarioRepository.GetModulosByUsuario(Convert.ToInt32(idUsuario));
                    if (modulosUsuario.Count() > 0)
                    {
                        var botoes = await _context.BotaoSideMenu.Where(b => b.IsAdmin).OrderBy(a => a.Sequencia).ToListAsync();
                        var botoesAux = botoes.Where(b => modulosUsuario.Contains(b.IdModulo)).ToList();
                        botoesAux.Add(botoes.FirstOrDefault(b => b.IsAdmin && b.Id == 23));
                        ViewBag.Botoes = botoesAux;
                    }
                }
                else
                {
                    ViewBag.Botoes = await _context.BotaoSideMenu.Where(b => b.IsAdmin).OrderBy(a => a.Sequencia).ToListAsync();
                }

            }


            //var usuarioRepository = context.HttpContext.RequestServices.GetService(typeof(UsuarioRepository)) as UsuarioRepository;

            //if (_idModulo > 0)
            //{
            //    acoesUsuario = usuarioRepository.GetAcoesUsuarioModulo(Convert.ToInt32(idUsuario), _idModulo).Result;
            //}

            //if (acoesUsuario == null || !acoesUsuario.Contains(_permissaoNecessaria))
            //{
            //    context.Result = new ViewResult()
            //    {
            //        ViewName = "AcessoNegado"
            //    };
            //}



            ViewBag.BotoesDropDown = await _context.BotaoDropDown.Include(a => a.BotaoSideMenu).ToListAsync();
            ViewBag.Perfil = idPerfil.ToString();
            return View();
        }

    }
}
