using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.ApiPagamento.Relatorios;
using SiteSesc.Models.Atividade;
using SiteSesc.Models.DB2;
using SiteSesc.Models.Enums;
using SiteSesc.Models.SiteViewModels;
using SiteSesc.Models.ViewModel;
using SiteSesc.Repositories;
using SiteSesc.Services;
using System.Globalization;
using System.Security.Claims;

namespace SiteSesc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("admin/atividades")]
    public class AtividadeOnLineController : BaseController
    {
        public readonly HostConfiguration hostConfiguration;
        private readonly IConfiguration configuration;
        private IWebHostEnvironment _env;
        private readonly AreaRepository _areaRepository;
        private ArquivoRepository _arquivoRepository;
        private AtividadeOnLineReposotory _atividadeRepository;
        private DefaultRepository _defaultRepository;
        //private readonly AtividadeOnLineReposotory _atividadeOnLineReposotory;
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly ClienteRepository _clienteRepository;
        private readonly LogRepository _logRepository;


        public AtividadeOnLineController(
            AtividadeOnLineReposotory atividadeRepository,
            AreaRepository areaRepository,
            IWebHostEnvironment env,
            IConfiguration configuration,
            UsuarioRepository usuarioRepository,
            ArquivoRepository arquivoRepository,
            DefaultRepository defaultRepository,
            ClienteRepository clienteRepository,
            CobrancaRepository cobrancaRepository,
            LogRepository logRepository)
        {
            this.configuration = configuration;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _usuarioRepository = usuarioRepository;
            _env = env;
            idModulo = (int)EnumModuloSistema.AtividadeOnLine;
            _arquivoRepository = arquivoRepository;
            _atividadeRepository = atividadeRepository;
            _cobrancaRepository = cobrancaRepository;
            _areaRepository = areaRepository;
            _defaultRepository = defaultRepository;
            _clienteRepository = clienteRepository;
            _logRepository = logRepository; 
        }

        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> Index(int? Inscrito, int pageNumber = 1, int pageSize = 20, int? idUop = null)
        {
            
            var paginatedList = await _atividadeRepository.GetPaginatedRecordsAsync(pageNumber, pageSize, idUop);
            var viewModel = new PaginatedListViewModel<AtividadeOnLine>
            {
                Items = paginatedList,
                PageIndex = paginatedList.PageIndex,
                TotalPages = paginatedList.TotalPages,
                HasPreviousPage = paginatedList.HasPreviousPage,
                HasNextPage = paginatedList.HasNextPage
            };
            if (idUop.HasValue)
            {
                ViewBag.UopSelecionada = await _defaultRepository.GetUopById((int)idUop);
                ViewBag.IdUop = idUop;
            }
            ViewBag.Uop = await _defaultRepository.ObtemUnidades();
            return View(viewModel);
        }

        //[PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.AtividadeOnLine)]
        //public async Task<IActionResult> Index(int? page, int? idTipoProcesso, string numeroProcesso)
        //{
        //    var atividade = await _atividadeRepository.Get();
        //    var usuario = await _usuarioRepository.GetUsuarioById(Convert.ToInt32(idUsuario));
        //    _logRepository.RegistrarLog("Visualização", "Atividades OnLine", usuario.Nome, 1, "Acesso painel de atividades online");
        //    return View(atividade);
        //}

        [Route("nova")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> Create(int? cduop, string cdelement, string validacao)
        {
            if (cduop != null)
            {
                var unidades = await _defaultRepository.ObtemUnidades();
                var atividades = await _atividadeRepository.ObterAtividadesSelect((int)cduop);

                ViewBag.Atividades = atividades.ToList();
                ViewBag.Unidades = unidades.ToList();
                ViewBag.CdUopSelect = unidades.FirstOrDefault(u => u.Cduop == cduop);

                ViewBag.Modalidade = await _areaRepository.GetSubArea();

                if (cdelement != null)
                {
                    if (atividades != null)
                    {
                        var atividade = atividades.FirstOrDefault(a => a.cdelement == cdelement);
                        var formapgto = await _atividadeRepository.ObterFormasPgtoCdelement(atividade!.cdelement);
                        var horarios = await _atividadeRepository.ObterHorariosCdelement(atividade!.cdelement);

                        atividade.formaspgto = formapgto.Where(a => !a.nmformato.Contains("ISEN")).ToList();
                        //atividade.horarios = horarios.ToList();

                        ViewBag.Atividade = atividade;
                        ViewBag.Unidades = unidades;
                        ViewBag.Validacao = validacao;

                        return View();
                    }
                }
                ViewBag.Validacao = validacao;
                return View();
            }
            ViewBag.Validacao = validacao;
            ViewBag.Unidades = await _defaultRepository.ObtemUnidades();
            return View();
        }

        [Route("inscritos")]
        public async Task<IActionResult> Inscritos(int cdprograma, int cdconfig, int sqocorrenc, string nome)
        {
            var turma = new Turma
            {
                CDPROGRAMA = cdprograma,
                CDCONFIG = cdconfig,
                SQOCORRENC = sqocorrenc
            };
            var usuario = await _usuarioRepository.ObterUsuarioPorCpf(cpf);
            ViewBag.NomeAtividade = nome;
            var atividadeDb2 = await _atividadeRepository.ObterAtividade(turma);
            ViewBag.UOP =  null;
            var inscritos = await _atividadeRepository.Inscritos(turma);
            if (inscritos != null && inscritos.Any())
            {
                var listaAlunos = new List<InscritoSituacaoCobranca>();
                foreach (var item in inscritos.Where(i => i.stinscri == 0))
                {
                    var clienteDb2 = await _clienteRepository.GetClienteCentralAtendimento(item.cduop, item.sqmatric);
                    if (clienteDb2 != null)
                    {
                        var cobrancas = await _cobrancaRepository.ObterCobrancaPorCpf(clienteDb2.NUCPF, DateTime.Now.Year);
                        if (cobrancas != null)
                        {
                            var cdelement = $"{turma.CDPROGRAMA.ToString().PadLeft(8, '0')}{turma.CDCONFIG.ToString().PadLeft(8, '0')}{turma.SQOCORRENC.ToString().PadLeft(8, '0')}";
                            var cobrancasAtividade = cobrancas.Where(c => c.CDELEMENT.Trim() == cdelement);
                            if (cobrancasAtividade != null)
                            {
                                var idadeCliente = (DateTime.Now.Date - item.dtnascimen.Date).Days / 365;
                                if (cobrancasAtividade.Any(c => c.STRECEBIDO == 0 && c.DTVENCTO.Date < DateTime.Now.Date))
                                {
                                    listaAlunos.Add(new InscritoSituacaoCobranca
                                    {
                                        cliente = clienteDb2,
                                        idade = idadeCliente,
                                        dtnascimento = item.dtnascimen,
                                        categoria = item.dscategori,
                                        dataInscricao = item.dtstatus,
                                        cdfonteinf = item.cdfonteinf,
                                        situação = "Inadimplente",
                                        contatos = item.contatos,
                                        classe = "danger"
                                    });
                                }
                                else
                                {
                                    listaAlunos.Add(new InscritoSituacaoCobranca
                                    {
                                        cliente = clienteDb2,
                                        idade = idadeCliente,
                                        dtnascimento = item.dtnascimen,
                                        categoria = item.dscategori,
                                        dataInscricao = item.dtstatus,
                                        cdfonteinf = item.cdfonteinf,
                                        situação = "Adimplente",
                                        contatos = item.contatos,
                                        classe = "success"
                                    });
                                }
                            }
                        }
                    }
                }

                _logRepository.RegistrarLog("Visualização", "Inscritos", usuario.Nome, 1, "Acesso ao Relatorio de inscritos por turma");
                return View(listaAlunos);
            }
            return View();
        }


        [HttpPost]
        [Route("nova")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> Create(AtividadeOnLine atividade, string imagemRecortada, int? cduop)
        {
            
            ModelState.Remove("Arquivo");
            ModelState.Remove("Usuario");
            ModelState.Remove("SubArea");
            ModelState.Remove("UnidadeOperacional");
            ModelState.Remove("Formulario");
            ModelState.Remove("IdAnamnese");
            if (cduop != null)
            {
                var uop = await _defaultRepository.GetUop((int)cduop);
                var arquivo = new Arquivo();
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (!await _atividadeRepository.AtvExist(atividade.cdelement))
                        {
                            if (!string.IsNullOrEmpty(imagemRecortada))
                                arquivo = await _arquivoRepository.ProcessImage(imagemRecortada, "atividade");


                            if (arquivo != null)
                            {
                                atividade.IdUnidadeOperacional = uop.Id;
                                atividade.IdArquivo = arquivo.Id;
                                atividade.IdUsuario = Convert.ToInt32(idUsuario);
                                var submit = await _atividadeRepository.Salvar(atividade);
                                if (submit == true)
                                    return RedirectToAction(nameof(Index), new { Inscrito = 2 });


                            }
                            ViewBag.Unidades = await _defaultRepository.ObtemUnidades();
                            return View(atividade);
                        }
                    }
                   
                    catch (Exception e)
                    {
                        return RedirectToAction(nameof(Index), new { Inscrito = 3 });

                    }
                }
            }
            ViewBag.Unidades = await _defaultRepository.ObtemUnidades();
            return RedirectToAction(nameof(Index), new { Inscrito = 1 });


        }

        [Route("editar-atividade-online")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var atividade = await _atividadeRepository.Get(id);
            if (atividade is not AtividadeOnLine)
                return NotFound();

            ViewBag.IdSubArea = new SelectList(await _areaRepository.GetSubArea(), "Id", "Nome", atividade.IdSubArea);
            ViewBag.AtividadeCentral = await _atividadeRepository.ObterAtividade(atividade.turma);
            return View(atividade);
        }

        [HttpPost]
        [Route("editar-atividade-online")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> Edit(AtividadeOnLine atividade, string imagemRecortada)
        {
            if (atividade == null)
                return NoContent();

            var arquivo = new Arquivo();
            if (!string.IsNullOrEmpty(imagemRecortada))
            {
                arquivo = await _arquivoRepository.ProcessImage(imagemRecortada, "atividade");
                atividade.IdArquivo = arquivo.Id;
            }

            // Usar o novo método Editar para evitar sobrescrever o IdUsuario
            var submit = await _atividadeRepository.Editar(atividade);
            if (submit == true)
                return RedirectToAction(nameof(Index));
            return View(atividade);
        }


        [Route("visualizar-pendentes")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> ValidarAtividade()
        {
            var atividadeOnLine = await _atividadeRepository.GetAtividadesPendentes();
            if (atividadeOnLine != null && atividadeOnLine.Any())
            {
                return View(atividadeOnLine);
            }
            return View(atividadeOnLine);
        }

        [Route("validar-atividade-online")]
        public async Task<IActionResult> EditarAtividade(int? id)
        {
            if (id == null)
                return NotFound();

            var atividade = await _atividadeRepository.Get(id);
            return View(atividade);
        }


        [HttpGet]
        [Route("alterar-status-atividade/{id}")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<JsonResult> AlterarStatusExibicao(int? id)
        {
            try
            {
                if (id != null)
                {
                    var desativa = await _atividadeRepository.AlteraStatus((int)id, false, Convert.ToInt32(idUsuario));
                    return Json(new
                    {
                        Code = 1,
                        Message = "Atividade removida"
                    });
                }
                return Json(new
                {
                    Code = 2,
                    Message = "Atividade não encontrada"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 0,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("validar-atividade/{id}")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.AtividadeOnLine)]
        public async Task<IActionResult> Validar(int? id)
        {
            try
            {
                if (id != null)
                {
                    var desativa = await _atividadeRepository.AlteraStatus((int)id, true);
                    return RedirectToAction(nameof(Index));
                }
                return Json(new
                {
                    Code = 2,
                    Message = "Atividade não encontrada"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 0,
                    Message = ex.Message
                });
            }
        }

        [Route("dashboard-atividade")]
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Relatorios)]
        public async Task<IActionResult> DashboardAtividade(string cdelement)
        {
            if (!string.IsNullOrEmpty(cdelement))
            {
                var turma = Util.CdelementToTurma(cdelement);
                var atividadeApi = await _atividadeRepository.ObterAtividade(turma);
                var inscritos = await _atividadeRepository.Inscritos(turma);
                var usuario = await _usuarioRepository.GetUsuarioById(Convert.ToInt32(idUsuario));

                var inadimplentes = await _atividadeRepository.Inadimplentes(cdelement);

                #region calcula numero de inadimplentes

                //if (inscritos != null)
                //{
                //    foreach (var item in inscritos)
                //    {
                //        var clienteDb2 = await _clienteRepository.GetClienteCentralAtendimento(item.cduop, item.sqmatric);
                //        var cobrancas = await _cobrancaRepository.ObterCobrancaPorCpf(clienteDb2.NUCPF, DateTime.Now.Year);
                //        if (cobrancas != null)
                //        {
                //            var cobrancasAtividade = cobrancas.Where(c => c.CDELEMENT.Trim() == cdelement);
                //            if (cobrancasAtividade != null)
                //            {
                //                if (cobrancasAtividade.Any(c => c.STRECEBIDO == 0 && c.DTVENCTO.Date < DateTime.Now.Date))
                //                {
                //                    inadimplentes++;
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion calcula numero de inadimplentes

                #region Calcula porcentagem ocupação

                var porcentagemOcupadas = (atividadeApi.nuvagasocp * 100) / atividadeApi.nuvagas;
                var porcentagemDisponiveis = 100 - porcentagemOcupadas;

                #endregion Calcula porcentagem ocupação

                ViewBag.PorcentagemOcupada = porcentagemOcupadas;
                ViewBag.PorcentagemDisponivel = porcentagemDisponiveis;
                ViewBag.Inadimplentes = inadimplentes;
                ViewBag.Evasoes = inscritos.Where(i => i.stinscri == 3);
                ViewBag.VagasTotais = atividadeApi.nuvagas;
                ViewBag.Inscritos = atividadeApi.nuvagasocp;
                ViewBag.VagasDisponiveis = atividadeApi.nuvagas - atividadeApi.nuvagasocp;
                _logRepository.RegistrarLog("Dashboard atividades", "Atividades", usuario.Nome, 1, "Acesso ao Dashboard da atividade " + atividadeApi.dsusuario);
                return View(atividadeApi);

            }
            //var usuario = await _usuarioRepository.ObterUsuarioPorCpf(cpf);
            //var atividade = await _atividadeOnLineReposotory.GetAtividadeId(id); // pega a atividade no banco do site

            //var cduop = Convert.ToInt32(atividade.UnidadeOperacional.Cduop);


            return View();


        }

        [Route("grafico-evasoes")]
        public async Task<JsonResult> GetInscricoesEvasoes(string cdelement)
        {
            var turma = Util.CdelementToTurma(cdelement);
            //var idAtividade = Convert.ToInt32(id);
            //var atividade = await _atividadeRepository.GetAtividadeId(idAtividade); // pega a atividade no banco do site

            //var turma = new Turma
            //{
            //    CDPROGRAMA = atividade.CdPrograma,
            //    CDCONFIG = atividade.CdConfig,
            //    SQOCORRENC = atividade.SqOcorrenc
            //};

            CultureInfo culture = new CultureInfo("pt-BR");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            var graficoEvaInsc = new List<GraficoInscricoesEvasoes>();
            var inscritos = await _atividadeRepository.Inscritos(turma);
            var groupData = inscritos.GroupBy(x => new { x.dtinscri.Month, x.dtinscri.Year }).ToList();
            foreach (var item in groupData)
            {
                var x = item.Select(a => a.stinscri);
                var inscricoesGrafico = x.Where(a => a == 0).Count();
                var evasoesGrafico = x.Where(a => a == 3).Count();
                var mesExtenso = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(item.Key.Month));
                var ordem = $"{item.Key.Month}{item.Key.Year}";
                graficoEvaInsc.Add(new GraficoInscricoesEvasoes
                {
                    mes = item.Key.Month,
                    ano = item.Key.Year,
                    mesAno = $"{mesExtenso.Substring(0, 3)}/{item.Key.Year}",
                    inscricoes = inscricoesGrafico,
                    evasoes = evasoesGrafico
                });
            }
            return Json(graficoEvaInsc.OrderBy(a => a.ano).ThenBy(a => a.mes));

            //return null;
        }
    }
}