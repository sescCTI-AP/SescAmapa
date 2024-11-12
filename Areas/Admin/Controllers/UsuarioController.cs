using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteSesc.Models;
using SiteSesc.Models.Admin;
using SiteSesc.Models.Enums;
using SiteSesc.Models.ViewModel;
using SiteSesc.Repositories;
using SiteSesc.Services;
using System.Drawing.Printing;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace SiteSesc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("admin/usuarios")]
    public class UsuarioController : BaseController
    {
        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            idModulo = (int)EnumModuloSistema.Usuario;
        }

        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Usuario)]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 100, string? busca = null) {
            var usuarios = new List<UsuarioList>();
            var usuarioLogado = await _usuarioRepository.ObterUsuarioPorCpf(cpf);

                var paginatedList = await _usuarioRepository.GetPaginatedRecordsAsync(pageNumber, pageSize, busca);

                var viewModel = new PaginatedListViewModel<UsuarioList>
                {
                    Items = paginatedList,
                    PageIndex = paginatedList.PageIndex,
                    TotalPages = paginatedList.TotalPages,
                    HasPreviousPage = paginatedList.HasPreviousPage,
                    HasNextPage = paginatedList.HasNextPage
                };

                return View(viewModel);

        }

        [HttpGet, Route("buscar-usuario")]
        public async Task<JsonResult> BuscarUsuario(string user)
        {
            if (!String.IsNullOrEmpty(user))
            {
                var usuarios = await _usuarioRepository.ObterUsuarioColaboradorPorNome(user);
                if (usuarios != null && usuarios.Any())
                {
                    return Json(new
                    {
                        Code = 1,
                        Lista = usuarios,
                    });
                }
                return Json(new
                {
                    Code = 2,
                });
            }

            return Json(new
            {
                Code = 0,
            });
        }

        [Route("detalhe")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Usuario)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)            
                return NotFound();

            var usuario = await _usuarioRepository.GetUsuarioById(id);
            if (usuario == null)            
                return NotFound();
            
            ViewData["IdPerfilUsuario"] = new SelectList(await _usuarioRepository.GetPerfilUsuario(), "Id", "Nome", usuario.IdPerfilUsuario);
            ViewBag.ModulosSistema = _usuarioRepository.GetModulos();
            ViewBag.AcoesSistema = _usuarioRepository.GetAcoes();
            return View(usuario);
        }

        [HttpPost]
        [Route("get-autorizacao")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Usuario)]
        public async Task<JsonResult> GetAutorizacaoModuloUsuario([FromBody] string id)
        {
            var usuario = await _usuarioRepository.GetUsuarioById(Convert.ToInt32(id));
            if (usuario != null)
            {
                var listaAutorizavoes = new List<ModuloAcoesSistemaUsuario>();
                if (usuario.adm_usuarioModuloSistema != null && usuario.adm_usuarioModuloSistema.Any())
                {
                    foreach (var item in usuario.adm_usuarioModuloSistema)
                    {
                        var autorizacao = new ModuloAcoesSistemaUsuario { IdModulo = item.IdModuloSistema };
                        var acoes = await _usuarioRepository.GetAcoesModulo(item.Id);
                        autorizacao.Acoes = acoes.Any()
                            ? autorizacao.Acoes = acoes.Select(a => a.IdAcaoSistema).ToArray()
                            : new int[] { };

                        listaAutorizavoes.Add(autorizacao);
                    }
                }
                return Json(new
                {
                    Code = 1,
                    Autorizacoes = listaAutorizavoes.Select(l => new ModuloAcoesSistemaUsuario
                    {
                        IdModulo = l.IdModulo,
                        Acoes = l.Acoes
                    }).ToList()
                });
            }
            return Json(new
            {
                Code = 2,
                Message = "Usuário não encontrado",
                ClassAlert = "warning"
            });
        }

        [HttpPost]
        [Route("atualizar")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Usuario)]
        public async Task<JsonResult> Atualizar([FromBody] AutorizacaoViewModel autorizacao, [FromServices] ClienteRepository clienteRepository)
        {
            var usuario = await _usuarioRepository.GetUsuarioCpf(autorizacao.cpf);
            var perfil = Convert.ToInt32(autorizacao.idPerfilUsuario);
            var usuarioLogado = await _usuarioRepository.GetUsuarioById(Convert.ToInt32(idUsuario));
            if (usuario != null)
            {
                if (usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.Coordernador && usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.SysAdmin && usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.Coordernador && usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.Funcionario)
                {
                    return Json(new
                    {
                        Code = 2,
                        Message = "Você não tem autorização para realizar essa ação!",
                        ClassAlert = "warning"
                    });
                }
                usuario.IsAtivo = autorizacao.isAtivo;
                usuario.IdPerfilUsuario = perfil;

                var atualiza = await _usuarioRepository.UpdateUsuario(usuario);

                //se o Perfil do usuário mudar para Admininstrador ou Cliente, as autorizações dos módulos/ações são removidas
                if (perfil == (int)PerfilUsuarioEnum.Administrador || perfil == (int)PerfilUsuarioEnum.Cliente || perfil == (int)PerfilUsuarioEnum.SysAdmin)
                {
                    var removeModulos = await _usuarioRepository.RemoveModulos(perfil, usuario);
                }

                return Json(new
                {
                    Code = 1,
                    Message = "Atualização realizada com sucesso",
                    ClassAlert = "success"
                });
            }
            return Json(new
            {
                Code = 2,
                Message = "Usuário não encontrado",
                ClassAlert = "warning"
            });
        }

        [HttpPost]
        [Route("acoes")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Usuario)]
        public async Task<JsonResult> AddAutorizacaoModuloUsuario([FromBody] SetAutorizacaoViewModel listaAutorizacao)
        {
            try{
                var salvaLista = await _usuarioRepository.AddAutorizacaoUsuario(listaAutorizacao);
                if(salvaLista)
                    return Json(new
                    {
                        Code = 1,
                        Message = "Autorizações cadastradas com sucesso",
                        ClassAlert = "success"
                    });
                return Json(new
                {
                    Code = 0,
                    Message = "Falha ao salvar lista",
                    ClassAlert = "error"
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Code = 0,
                    Message = e.Message,
                    ClassAlert = "error"
                });
            }           
        }

        [HttpPost]
        [Route("atualizar-email")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Usuario)]
        public async Task<JsonResult> AtualizarEmail([FromBody] AtualizacaoEmailViewModel atualizacaoEmail)
        {
            var usuario = await _usuarioRepository.GetUsuarioCpf(atualizacaoEmail.Cpf);

            if (usuario == null)
            {
                return new JsonResult(new
                {
                    Code = 2,
                    Message = "Usuário não encontrado",
                    ClassAlert = "warning"
                });
            }

            // Verifica se o usuário logado tem permissão para alterar o e-mail
            var usuarioLogado = await _usuarioRepository.GetUsuarioById(Convert.ToInt32(idUsuario));
            if (usuarioLogado == null || usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.SysAdmin && usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.Coordernador && usuarioLogado.IdPerfilUsuario != (int)PerfilUsuarioEnum.Funcionario)
            {
                return new JsonResult(new
                {
                    Code = 2,
                    Message = "Você não tem autorização para realizar essa ação!",
                    ClassAlert = "warning"
                });
            }

            // Atualiza o e-mail do usuário
            usuario.Email = atualizacaoEmail.NovoEmail;
            var atualizado = await _usuarioRepository.UpdateUsuario(usuario);

            if (atualizado)
            {
                return new JsonResult(new
                {
                    Code = 1,
                    Message = "E-mail atualizado com sucesso",
                    ClassAlert = "success"
                });
            }

            return new JsonResult(new
            {
                Code = 2,
                Message = "Erro ao atualizar o e-mail",
                ClassAlert = "warning"
            });
        }
    }
}
