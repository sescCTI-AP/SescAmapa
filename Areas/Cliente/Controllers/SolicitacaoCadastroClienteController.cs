using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using SiteSesc.Models;
using SiteSesc.Repositories;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using SiteSesc.Models.ViewModel;
using System.ComponentModel.DataAnnotations.Schema;
using SiteSesc.Models.ApiPagamento;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SiteSesc.Services;

namespace SiteSesc.Areas.Cliente.Controllers
{
    [Authorize]
    [Area("Cliente")]
    [Route("cadastro")]
    public class SolicitacaoCadastroClienteController : Controller
    {
        private SiteSescContext _context;
        public readonly HostConfiguration hostConfiguration;
        private int perfil;
        private string idUsuario;
        private string guid;
        private string cpf;
        private IWebHostEnvironment _env;
        private ParentescoRepository _parentescoRepository;
        private DefaultRepository _defaultRepository;
        private ArquivoRepository _arquivoRepository;
        private SolicitacaoCadastroRepository _solicitacaoRepository;
        private ClienteRepository _clienteRepository;

        public SolicitacaoCadastroClienteController(SiteSescContext context,
            IWebHostEnvironment env,
            IConfiguration configuration,
            ParentescoRepository parentescoRepository,
            DefaultRepository defaultRepository,
            ArquivoRepository arquivoRepository,
            SolicitacaoCadastroRepository solicitacaoCadastroRepository,
            ClienteRepository clienteRepository)
        {
            _context = context;
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _env = env;
            _parentescoRepository = parentescoRepository;
            _defaultRepository = defaultRepository;
            _arquivoRepository = arquivoRepository;
            _solicitacaoRepository = solicitacaoCadastroRepository;
            _clienteRepository = clienteRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            perfil = int.Parse(((ClaimsIdentity)User.Identity).FindFirst("Perfil").Value);
            idUsuario = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;
            cpf = ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
            guid = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Sid).Value;
        }

        [Route("escolha-categoria")]
        public IActionResult EscolhaCategoria(string? message = null)
        {

            var modelStateErrors = TempData["ModelState"] as Dictionary<string, string>;
            if (modelStateErrors != null)
            {
                foreach (var error in modelStateErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            return View(modelStateErrors);
        }


        [Route("novo")]
        public async Task<ActionResult> Create(int categoria, int? CDUOTITUL = null, int? SQTITULMAT = null)
        {
            if (idUsuario == null)
                return RedirectToAction("Login", "Home");

            ViewBag.Comerciario = categoria == 0 ? true : false;
            //ViewData["IdParentesco"] = new SelectList(await _parentescoRepository.GetParentescos(), "Id", "Nome");
            ViewData["IdSexo"] = new SelectList(await _defaultRepository.GetSexo(), "Id", "Nome");
            ViewData["IdEstadoCivil"] = new SelectList(await _defaultRepository.GetEstadoCivil(), "Id", "Nome");
            ViewData["IdEscolaridade"] = new SelectList(await _defaultRepository.GetEscolaridade(), "Id", "Nome");
            ViewData["IdSituacaoProfissional"] = new SelectList(await _defaultRepository.GetSituacaoProfissional(), "Id", "Nome");
            ViewData["IdTipoDocumentoIdentificacao"] = new SelectList(await _defaultRepository.GetTipoDocumentoIdentificacao(), "Id", "Nome");
            ViewData["Municipios"] = new SelectList(await _clienteRepository.GetMunicipios(), "CDMUNICIP", "DSMUNICIP");
            if (CDUOTITUL != null)
                ViewBag.CDUOTITUL = CDUOTITUL;
            if (SQTITULMAT != null)
                ViewBag.SQTITULMAT = SQTITULMAT;
            return View();
        }

        [Route("renovacao")]
        public async Task<ActionResult> Renovacao(int CDUOP, int SQMATRIC)
        {
            if (idUsuario == null)
                return RedirectToAction("Login", "Home");
            var categoriasPleno = Util.CategoriasPleno();
            var clienteCentral = await _clienteRepository.GetClienteCentralAtendimento(CDUOP, SQMATRIC);
            if (clienteCentral != null)
            {
                ViewBag.Comerciario = categoriasPleno.Contains(clienteCentral.CDCATEGORI)  ? true : false;
                //ViewData["IdParentesco"] = new SelectList(await _parentescoRepository.GetParentescos(), "Id", "Nome");
                ViewBag.ClienteCentral = clienteCentral;
                ViewData["IdSexo"] = new SelectList(await _defaultRepository.GetSexo(), "Id", "Nome");
                ViewData["IdEstadoCivil"] = new SelectList(await _defaultRepository.GetEstadoCivil(), "Id", "Nome");
                ViewData["IdEscolaridade"] = new SelectList(await _defaultRepository.GetEscolaridade(), "Id", "Nome");
                ViewData["IdSituacaoProfissional"] = new SelectList(await _defaultRepository.GetSituacaoProfissional(), "Id", "Nome");
                ViewData["IdTipoDocumentoIdentificacao"] = new SelectList(await _defaultRepository.GetTipoDocumentoIdentificacao(), "Id", "Nome");
                ViewData["Municipios"] = new SelectList(await _clienteRepository.GetMunicipios(), "CDMUNICIP", "DSMUNICIP");
                if (clienteCentral.CDUOTITUL != null)
                    ViewBag.CDUOTITUL = clienteCentral.CDUOTITUL;
                if (clienteCentral.SQTITULMAT != null)
                    ViewBag.SQTITULMAT = clienteCentral.SQTITULMAT;

                var clienteView = SolicitacaoCadastroCliente.ToSolicitacaoCadastro(clienteCentral);
                return View(clienteView);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("novo-submit")]
        public async Task<ActionResult> CreatePost(SolicitacaoCadastroCliente solicitacao, int? CDUOTITUL = null, int? SQTITULMAT = null, string? cpfDependente = null)
        {
            try
            {
                #region Remove State
                ModelState.Remove("Usuario");
                ModelState.Remove("Escolaridade");
                ModelState.Remove("EstadoCivil");
                ModelState.Remove("Parentesco");
                ModelState.Remove("SituacaoProfissional");
                ModelState.Remove("Sexo");
                ModelState.Remove("Status");
                ModelState.Remove("TipoDocumentoIdentificacao");
                ModelState.Remove("HstSolicitacao");
                ModelState.Remove("RendaFamiliar");
                ModelState.Remove("RendaIndividual");
                if (!string.IsNullOrEmpty(cpfDependente))
                {
                    ModelState.Remove("FotoPerfil");
                    ModelState.Remove("VinculoPrimeiraPagina");
                    ModelState.Remove("VinculoSegundaPagina");
                    ModelState.Remove("ContraCheque");
                    ModelState.Remove("ComprovanteEndereco");

                }
                if (solicitacao.TipoCategoria.ToLower() != "pleno")
                {
                    ModelState.Remove("FotoPerfil");
                    ModelState.Remove("ComprovanteEndereco");
                    ModelState.Remove("DocumentoCpf");
                    ModelState.Remove("RgFrente");
                    ModelState.Remove("RgVerso");
                    ModelState.Remove("VinculoPrimeiraPagina");
                    ModelState.Remove("VinculoSegundaPagina");
                    ModelState.Remove("ContraCheque");
                }
  //              solicitacao.RendaFamiliar = 0;
  //              solicitacao.RendaIndividual = 0;
                #endregion
                if (ModelState.IsValid)
                {
                    if (solicitacao.TipoCategoria.ToLower() != "pleno" && solicitacao.TipoCategoria.ToLower() != "renovacao")
                    {

                        if (solicitacao.FotoPerfil != null)
                        {
                            var foto = "";
                            using (var memoryStream = new MemoryStream())
                            {
                                solicitacao.FotoPerfil.CopyTo(memoryStream);
                                var fileBytes = memoryStream.ToArray();
                                foto = Convert.ToBase64String(fileBytes);

                            }
                            var cpfCadastro = cpf;
                            if (!string.IsNullOrEmpty(cpfDependente))
                                cpfCadastro = cpfDependente;

                            var clienteAdd = SolicitacaoCadastroCliente.ToClienteAdd(solicitacao, cpfCadastro);
                            if (clienteAdd != null)
                            {
                                if (CDUOTITUL != null)
                                    clienteAdd.CDUOTITUL = CDUOTITUL;
                                if (SQTITULMAT != null)
                                    clienteAdd.SQTITULMAT = SQTITULMAT;

                                clienteAdd.FOTO64 = foto;
                                //Salva cliente no db2
                                var cliente = await _clienteRepository.AddClienteDb2(clienteAdd);
                                return RedirectToAction("Index", "Cliente");
                            }
                        }
                        return RedirectToAction("EscolhaCategoria");
                    }
                    else
                    {
                        var arquivos = _arquivoRepository.GetIFormFileFields(solicitacao);
                        if (!string.IsNullOrEmpty(cpfDependente))
                        {
                            if (Util.ValidaCpf(cpfDependente))
                            {
                                cpfDependente = cpfDependente.Replace(".", "").Replace("-", "");
                                if (CDUOTITUL != null)
                                    solicitacao.Cduotitul = CDUOTITUL;
                                if (SQTITULMAT != null)
                                {
                                    solicitacao.Sqtitulmat = SQTITULMAT;
                                    solicitacao.Cpf = cpfDependente;
                                    if (!string.IsNullOrEmpty(cpfDependente))
                                        cpf = cpfDependente;
                                }
                            }
                            else
                            {
                                return RedirectToAction("Index", "Cliente");
                            }
                        }

                        //verifica se existe solicitaçao já criada. Caso sim, retorna para a tela inicial
                        if (_solicitacaoRepository.VerificarExisteSolicitacao(cpf, solicitacao.TipoCategoria.ToLower()).Result)
                        {                           
                            return RedirectToAction("Index", "Cliente");

                        }
                        solicitacao.IdUsuario = Convert.ToInt32(idUsuario);
                        if (!solicitacao.IsDependente && !string.IsNullOrEmpty(solicitacao.CnpjEmpresa))
                        {
                            if (solicitacao.CnpjEmpresa.Contains('.') )
                            {
                                solicitacao.CnpjEmpresa = Regex.Replace(solicitacao.CnpjEmpresa, @"[^0-9]+", "");
                            }

                        }
                                         
                        var submit = await _solicitacaoRepository.Save(solicitacao);
                        var saveDocCliente = await _arquivoRepository.SaveClientFile(arquivos, solicitacao, cpf);
                    }

                    return RedirectToAction("Index", "Cliente");
                }
                if (!ModelState.IsValid)
                {
                    TempData["ModelState"] = ModelStateToDictionary(ModelState);
                    return RedirectToAction("EscolhaCategoria");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("EscolhaCategoria", new { message = e.Message });
            }
            ViewData["IdSexo"] = new SelectList(await _defaultRepository.GetSexo(), "Id", "Nome", solicitacao.IdSexo);
            ViewData["IdEstadoCivil"] = new SelectList(await _defaultRepository.GetEstadoCivil(), "Id", "Nome", solicitacao.IdEstadoCivil);
            ViewData["IdEscolaridade"] = new SelectList(await _defaultRepository.GetEscolaridade(), "Id", "Nome", solicitacao.IdEscolaridade);
            ViewData["IdSituacaoProfissional"] = new SelectList(await _defaultRepository.GetSituacaoProfissional(), "Id", "Nome", solicitacao.IdSituacaoProfissional);
            ViewData["IdTipoDocumentoIdentificacao"] = new SelectList(await _defaultRepository.GetTipoDocumentoIdentificacao(), "Id", "Nome", solicitacao.IdTipoDocumentoIdentificacao);
            return RedirectToAction("EscolhaCategoria", new { message = "3" });
        }

        private Dictionary<string, string> ModelStateToDictionary(ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string>();
            foreach (var state in modelState)
            {
                if (state.Value.Errors.Any())
                {
                    errors[state.Key] = state.Value.Errors.FirstOrDefault()?.ErrorMessage;
                }
            }
            return errors;
        }

        [Route("editar/{id}")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Cliente", new { area = "Cliente" });
            }
            var solicitacao = await _solicitacaoRepository.GetSoliticacao(id);
            if (solicitacao != null)
            {
                if (solicitacao.Cduotitul != null)
                {

                    ViewBag.CDUOTITUL = solicitacao.Cduotitul;

                }
                ViewData["IdSexo"] = new SelectList(await _defaultRepository.GetSexo(), "Id", "Nome", solicitacao.IdSexo);
                ViewData["IdEstadoCivil"] = new SelectList(await _defaultRepository.GetEstadoCivil(), "Id", "Nome", solicitacao.IdEstadoCivil);
                ViewData["IdEscolaridade"] = new SelectList(await _defaultRepository.GetEscolaridade(), "Id", "Nome", solicitacao.IdEscolaridade);
                ViewData["IdSituacaoProfissional"] = new SelectList(await _defaultRepository.GetSituacaoProfissional(), "Id", "Nome", solicitacao.IdSituacaoProfissional);
                ViewData["IdTipoDocumentoIdentificacao"] = new SelectList(await _defaultRepository.GetTipoDocumentoIdentificacao(), "Id", "Nome", solicitacao.IdTipoDocumentoIdentificacao);
                ViewBag.Arquivos = await _solicitacaoRepository.GetArquivosSolicitacao(id);
                return View(solicitacao);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("editar/{id}")]
        public async Task<IActionResult> Edit(int id, SolicitacaoCadastroCliente solicitacao)
        {
            try
            {
                var arquivosUpload = new List<IFormFile>();
                var arquivos = _arquivoRepository.GetIFormFileFields(solicitacao);
                if (arquivos.Any(a => a != null))
                    arquivosUpload.AddRange(arquivos.Where(a => a != null));

                //pesquisa arquivos em [ArquivoCadastroCliente] pela descrição dos arquivos enviados - ok
                //adiciona novos arquivos - ok
                //remove arquivos antigos - ok

                //Adicionar registro de historico de solicitacao
                //Alterar status da solicitacao para "em análise"
                var editArquivos = await _arquivoRepository.EditArquvos(arquivosUpload, solicitacao);

                var hst = new HstSolicitacao
                {
                    Observacao = "Cliente reenviou formulário para análise",
                    IsCliente = true,
                    IdUsuario = Convert.ToInt32(idUsuario),
                    IdSolicitacaoCadastroCliente = solicitacao.Id
                };
                var historico = await _solicitacaoRepository.AddHistorico(hst, 1);

                return RedirectToAction("Index", "Cliente", new { area = "Cliente" });
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [Route("busca-cnpj")]
        public async Task<JsonResult> GetDadosEmpresaByCnpj(string cnpj)

        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length < 18)
            {
                return Json(new { Code = 3, Message = "Informe o CNPJ" });
            }

            var empresa = await Util.GetDadosEmpresaByCnpj(cnpj);
            if (empresa != null)
            {
                return Json(new
                {
                    Code = 1,
                    Data = new
                    {
                        RazaoSocial = empresa.nome,
                    }
                });
            }
            return Json(new { Code = 2, Message = $"Falha ao pesquisar o CNPJ. Se preferir, informe os dados manualmente." });


        }

        [Route("busca-cep/{cep}")]
        public async Task<JsonResult> GetEndereco(string cep)

            {
            if (string.IsNullOrEmpty(cep))
            {
                return Json(new { Code = 3, Message = "Informe o CNPJ" });
            }
            try
            {
                cep = Regex.Replace(cep, @"[^0-9]+", "");
                var client = new HttpClient();
                var result = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
                if (result.IsSuccessStatusCode)
                {
                    var consultaCep = JsonConvert.DeserializeObject<ConsultaCep>(result.Content.ReadAsStringAsync().Result);
                    return Json(new
                    {
                        Code = 1,
                        Data = consultaCep
                    });
                }
            }
            catch (Exception e)
            {
                return Json(new { Code = 2, Message = $"Falha ao buscar CEP." });
            }
            return null;
        }

    }
}
