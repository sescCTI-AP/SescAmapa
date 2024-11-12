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
    [Route("admin/area")]
    public class AreaController : BaseController
    {
        private readonly SiteSescContext _context;
        private ArquivoRepository _arquivoRepository;
        public AreaController(SiteSescContext context, UsuarioRepository usuarioRepository, ArquivoRepository arquivoRepository)
        {
            _context = context;
            idModulo = (int)EnumModuloSistema.Area;
            _usuarioRepository = usuarioRepository;
            _arquivoRepository = arquivoRepository;
        }

        // GET: Admin/Area
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Area.ToListAsync());
        }

        // GET: Admin/Area/Create
        [Route("novo")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Area)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("nova-area")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> CreatePost(Area area)
        {
            if (ModelState.IsValid)
            {
                _context.Add(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        // GET: Admin/Area/Edit/5
        [Route("editar")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Area.Include(a => a.Arquivo).SingleOrDefaultAsync(a => a.Id == id);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        [HttpPost]
        [Route("editar-post")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> EditPost(int? id, string nome, string Descricao)
        {

            try
            {
                if (!string.IsNullOrEmpty(nome))
                {
                    var area = await _context.Area.FindAsync(id);
                    area.Nome = nome;
                    area.Descricao = Descricao;
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                throw;

            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/Area/Delete/5
        [Route("excluir")]
        [PermissionFilter((int)Permissao.Deletar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Area
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // POST: Admin/Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("excluir")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Deletar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await _context.Area.FindAsync(id);
            if (area != null)
            {
                _context.Area.Remove(area);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("anexar-arquivo")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Area)]
        public async Task<IActionResult> Anexar(int? id, string imagemRecortada)
        {
            var arquivo = new Arquivo();
            try
            {
                if (!string.IsNullOrEmpty(imagemRecortada))
                {
                    arquivo = await _arquivoRepository.ProcessImage(imagemRecortada, "area");
                }

                if (arquivo != null)
                {
                    var area = await _context.Area.FindAsync(id);
                    area.IdArquivo = arquivo.Id;
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction("Edit", new { id = id });
        }


        private bool AreaExists(int id)
        {
            return _context.Area.Any(e => e.Id == id);
        }
    }
}
