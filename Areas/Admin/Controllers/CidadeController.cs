using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.Enums;
using SiteSesc.Repositories;
using SiteSesc.Services;

namespace SiteSesc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("admin/cidades")]

    public class CidadeController : BaseController
    {
        private readonly SiteSescContext _context;

        public CidadeController(SiteSescContext context, UsuarioRepository usuarioRepository)
        {
            _context = context;
            idModulo = (int)EnumModuloSistema.Cidades;
            _usuarioRepository = usuarioRepository;
        }

        // GET: Admin/Cidade
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cidade.ToListAsync());
        }

        // GET: Admin/Cidade/Details/5
        [Route("detlhes/{id}")]
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cidade = await _context.Cidade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cidade == null)
            {
                return NotFound();
            }

            return View();
        }

        // GET: Admin/Cidade/Create
        [Route("novo")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Cidades)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("novo")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> Create(Cidade cidade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cidade);
        }

        // GET: Admin/Cidade/Edit/5
        [Route("editar/{id}")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cidade = await _context.Cidade.FindAsync(id);
            if (cidade == null)
            {
                return NotFound();
            }
            return View(cidade);
        }

        [HttpPost]
        [Route("editar")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> EditPost(int id, Cidade cidade)
        {
            if (id != cidade.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cidade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CidadeExists(cidade.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cidade);
        }

        // GET: Admin/Cidade/Delete/5
        [Route("excluir/{id}")]
        [PermissionFilter((int)Permissao.Deletar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cidade = await _context.Cidade.FirstOrDefaultAsync(m => m.Id == id);
            if (cidade == null)
                return NotFound();

            return View(cidade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("excluir/{id}")]
        [PermissionFilter((int)Permissao.Deletar, (int)EnumModuloSistema.Cidades)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cidade = await _context.Cidade.FindAsync(id);
            if (cidade != null)            
                _context.Cidade.Remove(cidade);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CidadeExists(int id)
        {
            return _context.Cidade.Any(e => e.Id == id);
        }


        [HttpPost]
        [Route("status")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Cidades)]
        public async Task<JsonResult> AlterarStatusExibicao([FromBody] int id)
        {
            try
            {
                var reg = await _context.Cidade.FindAsync(id);
                if (reg != null)
                {
                    reg.IsAtivo = !reg.IsAtivo;
                    _context.Update(reg);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        Code = 1,
                        Message = "Status de exibição alterado com sucesso"
                    }); //sucesso
                }
                return Json(new
                {
                    Code = 2,
                    Message = "Item não encontrado"
                });//não encontrado
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 0,
                    Message = ex.Message
                });//erro
            }
        }
    }
}
