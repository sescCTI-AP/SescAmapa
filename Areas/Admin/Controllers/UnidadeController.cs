using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
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
    [Route("admin/unidades")]

    public class UnidadeController : BaseController
    {
        private readonly SiteSescContext _context;

        private ArquivoRepository _arquivoRepository;

        public UnidadeController(SiteSescContext context, UsuarioRepository usuarioRepository, ArquivoRepository arquivoRepository)
        {
            _context = context;
            idModulo = (int)EnumModuloSistema.Unidade;
            _usuarioRepository = usuarioRepository;
            _arquivoRepository = arquivoRepository;
        }

        // GET: Admin/Unidade
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.UnidadeOperacional.ToListAsync());
        }

        // GET: Admin/Unidade/Details/5
        [Route("detalhes/{id}")]
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unidade = await _context.UnidadeOperacional
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unidade == null)
            {
                return NotFound();
            }

            return View(unidade);
        }

        // GET: Admin/Unidade/Create
        [Route("novo")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Unidade)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("novo")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Create(UnidadeOperacional unidade)
        {
            ModelState.Remove("Usuario");
            ModelState.Remove("cdp_Cardapio");
            ModelState.Remove("FeriadoEscolarUnidade");
            ModelState.Remove("ProcessoAgendamentoEscolar");
            ModelState.Remove("EventoAvulso");
            ModelState.Remove("Noticia");
            ModelState.Remove("Evento");
            ModelState.Remove("Arquivo");
            ModelState.Remove("NameRoute");


            unidade.IdUsuario = 1;
            unidade.IdArquivo = 1;
            unidade.DataCadastro = DateTime.Now;
            unidade.DataAtualizacao = DateTime.Now;
            unidade.NameRoute = GerarSlug(unidade.NomeCurto);

            if (ModelState.IsValid)
            {
                
                _context.Add(unidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unidade);
        }

        private string GerarSlug(string nomeCurto)
        {
            if (string.IsNullOrEmpty(nomeCurto))
                return string.Empty;

            // Substitui acentos e caracteres especiais
            var acentuados = new Dictionary<char, string> {
            { 'ã', "a" }, { 'á', "a" }, { 'à', "a" }, { 'â', "a" }, { 'ä', "a" },
            { 'é', "e" }, { 'ê', "e" }, { 'í', "i" },
            { 'ó', "o" }, { 'õ', "o" }, { 'ô', "o" }, { 'ö', "o" },
            { 'ú', "u" }, { 'ü', "u" }, { 'ç', "c" }
        };

            string nomeSemAcento = new string(nomeCurto
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Select(c => acentuados.ContainsKey(c) ? acentuados[c][0] : c)
                .ToArray())
                .Normalize(NormalizationForm.FormC);

            // Gera o slug
            string slug = Regex.Replace(nomeSemAcento.ToLower(), @"\s+", "-"); // Substitui espaços por hífens
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", ""); // Remove caracteres especiais

            return slug;
        }

        // GET: Admin/Unidade/Edit/5
        [Route("editar/{id}")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unidade = await _context.UnidadeOperacional.FindAsync(id);
            if (unidade == null)
            {
                return NotFound();
            }
            return View(unidade);
        }

        [HttpPost]
        [Route("editar/{id}")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Edit(int id, UnidadeOperacional unidade)
        {
            if (id != unidade.Id)
                return NotFound();

            // Carregar a unidade existente do banco de dados para preservar os campos que você não deseja alterar
            var unidadeExistente = await _context.UnidadeOperacional.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            if (unidadeExistente == null)
                return NotFound();

            // Remover validações desnecessárias
            ModelState.Remove("Usuario");
            ModelState.Remove("cdp_Cardapio");
            ModelState.Remove("FeriadoEscolarUnidade");
            ModelState.Remove("ProcessoAgendamentoEscolar");
            ModelState.Remove("EventoAvulso");
            ModelState.Remove("Noticia");
            ModelState.Remove("Evento");
            ModelState.Remove("Arquivo");

            // Preservar os valores que não estão sendo alterados
            unidade.DataCadastro = unidadeExistente.DataCadastro;  // Preservar data de cadastro
            unidade.IdArquivo = unidadeExistente.IdArquivo;        // Preservar IdArquivo existente
            unidade.DataAtualizacao = DateTime.Now;        // Atualizar DataAtualizacao

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unidade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnidadeExists(unidade.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unidade);
        }


        // GET: Admin/Unidade/Delete/5
        [Route("excluir/{id}")]
        [PermissionFilter((int)Permissao.Deletar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var unidade = await _context.UnidadeOperacional.FirstOrDefaultAsync(m => m.Id == id);
            if (unidade == null)
                return NotFound();
                
            return View(unidade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("excluir/{id}")]
        [PermissionFilter((int)Permissao.Deletar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unidade = await _context.UnidadeOperacional.FindAsync(id);
            if (unidade != null)            
                _context.UnidadeOperacional.Remove(unidade);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnidadeExists(int id)
        {
            return _context.UnidadeOperacional.Any(e => e.Id == id);
        }


        [HttpPost]
        [Route("anexar-arquivo")]
        [ValidateAntiForgeryToken]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Unidade)]
        public async Task<IActionResult> Anexar(int? id, string imagemRecortada)
        {
            var arquivo = new Arquivo();
            try
            {
                if (!string.IsNullOrEmpty(imagemRecortada))
                {
                    arquivo = await _arquivoRepository.ProcessImage(imagemRecortada, "unidades");
                }

                if (arquivo != null)
                {
                    var unidade = await _context.UnidadeOperacional.FindAsync(id);
                    unidade.IdArquivo = arquivo.Id;
                    _context.Update(unidade);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        [Route("status")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Unidade)]
        public async Task<JsonResult> AlterarStatusExibicao([FromBody] int id)
        {
            try
            {
                var reg = await _context.UnidadeOperacional.FindAsync(id);
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
