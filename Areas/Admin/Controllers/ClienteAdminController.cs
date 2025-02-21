using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models;
using SiteSesc.Repositories;
using SiteSesc.Models.Enums;
using SiteSesc.Services;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Helpers;
using SiteSesc.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Newtonsoft.Json;
using static SiteSesc.Models.Status;

namespace SiteSesc.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("admin/clientes")]
    public class ClienteAdminController : BaseController
    {
        private SolicitacaoCadastroRepository _solicitacaoCadastroRepository;
        private MensagemRapidaRepository _mensagemRapidaRepository;
        private ClienteRepository _clienteRepository;
        private readonly EmailService _emailService;
        private IWebHostEnvironment _server;


        public ClienteAdminController(SolicitacaoCadastroRepository solicitacaoCadastroRepository,
            EmailService emailService,
                        IWebHostEnvironment server,
            MensagemRapidaRepository mensagemRapidaRepository,
            UsuarioRepository usuarioRepository,
            ClienteRepository clienteRepository)
        {
            _solicitacaoCadastroRepository = solicitacaoCadastroRepository;
            _mensagemRapidaRepository = mensagemRapidaRepository;
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _emailService = emailService;
            _server = server;
            idModulo = (int)EnumModuloSistema.Clientes;
        }

        #region Dashboard
        [Route("dashboard")]
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Clientes)]
        public async Task<IActionResult> Dashboard()
        {
            var solicitacoes = await _solicitacaoCadastroRepository.GetAllSoliticacoes();
            ViewBag.Recebidas = solicitacoes.Count();
            ViewBag.Atendidas = _solicitacaoCadastroRepository.GetTotalFinalizadas();
            ViewBag.Pleno = _solicitacaoCadastroRepository.GetTotaPleno();
            ViewBag.Atividades = _solicitacaoCadastroRepository.GetTotaAtividades();
            ViewBag.Correcoes = _solicitacaoCadastroRepository.GetTotalCorrecoes();
            ViewBag.Novas = _solicitacaoCadastroRepository.GetNovasSolicitacoes();
            return View(solicitacoes);
        }
        #endregion

        ///[Route("soliticacoes-cadastro")]
        //[PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Clientes)]
        //public async Task<IActionResult> SolicitacaoCadastroCliente(int? status = 1, string? validacao = null)
        //{

        //    ViewBag.Validacao = validacao;
        //    var listaFiltro = Filtro.GetLista();
        //    var solicitacoes = await _solicitacaoCadastroRepository.GetAllSoliticacoes(status);


        //    ViewBag.status = new SelectList(listaFiltro, "Id", "Nome", status);
        //    return View(solicitacoes);
        //}

        [Route("soliticacoes-cadastro")]
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Clientes)]
        public async Task<IActionResult> SolicitacaoCadastroCliente(int? idStatus = 1)
        {
            // Obtém todas as solicitações com o filtro de status
            var solicitacoes = await _solicitacaoCadastroRepository.GetAllSoliticacoes(idStatus);

            // Ordena as solicitações pelo número de dias úteis entre a data de solicitação e a data atual
            var solicitacoesOrdenadas = solicitacoes
                .Select(solicitacao => new
                {
                    Solicitacao = solicitacao,
                    DiasEmAnalise = Util.GetQuantidadeDiasUteisEntreDatas(solicitacao.DataSolicitacao)
                })
                .OrderByDescending(s => s.DiasEmAnalise) // Ordena de forma decrescente pelos dias em análise
                .Select(s => s.Solicitacao) // Retorna apenas as solicitações (sem o campo extra de dias)
                .ToList();
            
            var listaFiltro = Filtro.GetLista();
            ViewBag.idStatus = new SelectList(listaFiltro, "Id", "Nome", idStatus);

            return View(solicitacoesOrdenadas);
        }



        [Route("detalhes-solicitacao/{id}")]
        [PermissionFilter((int)Permissao.Visualizar, (int)EnumModuloSistema.Clientes)]
        public async Task<IActionResult> Details(Guid? id, string? validacao = null)
        {
            if (id == null)
            {
                return RedirectToAction("SolicitacaoCadastroCliente");
            }
            var solicitacao = await _solicitacaoCadastroRepository.GetSoliticacao(id);
            if (solicitacao.Cduotitul != null && solicitacao.Sqtitulmat != null)
            {
                ViewBag.Titular = await _clienteRepository.GetClienteCentralAtendimento((int)solicitacao.Cduotitul, (int)solicitacao.Sqtitulmat);
                
            }
            var arquivosSolicitacao = await _solicitacaoCadastroRepository.GetArquivosSolicitacao(id);
            
            ViewBag.MsgRapidas = await _mensagemRapidaRepository.GetMensagensRapidas();
            ViewBag.Arquivos = arquivosSolicitacao;
            ViewBag.Historico = await _solicitacaoCadastroRepository.GetHistorico(id);
            return View(solicitacao);
        }

        [Route("add/{id}")]
        [PermissionFilter((int)Permissao.Cadastrar, (int)EnumModuloSistema.Clientes)]
        public async Task<IActionResult> AddCliente(Guid id)
        {
            
            var solicitacao = await _solicitacaoCadastroRepository.GetSoliticacao(id);

            var cpfCadastro = string.IsNullOrEmpty(solicitacao.Cpf) ? solicitacao.Usuario.Cpf : solicitacao.Cpf;
            var cliente = Models.SolicitacaoCadastroCliente.ToClienteAdd(solicitacao, cpfCadastro);

            try
            {

                if (cliente != null)
                {
                    var arquivosSolicitacao = await _solicitacaoCadastroRepository.GetArquivosSolicitacao(id);
                    var fotoPerfil = arquivosSolicitacao.FirstOrDefault(a => a.Tipo == "FotoPerfil").Arquivo.CaminhoVirtualFormatado();
                    var foto = await Util.DownloadImageAsBase64(fotoPerfil);
                    cliente.FOTO64 = foto;
                   
                    var responseJson = await _clienteRepository.AddClienteDb2(cliente);
                    
                    if (responseJson?.success == true)
                    {
                        
                        var cli  = JsonConvert.DeserializeObject<ClienteAdd>(JsonConvert.SerializeObject(responseJson.data));
                        
                        var hst = new HstSolicitacao
                        {
                            Observacao = "Cliente adicionado na central de atendimento.",
                            IsCliente = false,
                            IdUsuario = Convert.ToInt32(idUsuario),
                            IdSolicitacaoCadastroCliente = solicitacao.Id
                        };
                        await _solicitacaoCadastroRepository.AddHistorico(hst, 5);
                        
                        return Json(new { success = true, message = responseJson.message });
                    }
                    
                    string errorMessage = responseJson?.error?.ToString() ?? "Erro desconhecido";
                    return Json(new { success = false, error = errorMessage });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = $"Erro inesperado na API" });
            }
            return Json(new { success = false, error = "Algo de errado não está certo" });
        }

        [HttpPost]
        [Route("envia-correcao")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Clientes)]
        public async Task<IActionResult> EnviaCorrecao(string Id, string msg)
        {
            if (string.IsNullOrEmpty(Id))
                return Json(new { code = 0, msg = "Falha ao salvar interação" });

            var solicitacao = await _solicitacaoCadastroRepository.GetSoliticacao(new Guid(Id));
            if (solicitacao != null)
            {
                if (solicitacao.IdStatus <= 4 || solicitacao.IdStatus == 9)
                {
                    var hst = new HstSolicitacao
                    {
                        Observacao = msg,
                        IsCliente = false,
                        IdUsuario = Convert.ToInt32(idUsuario),
                        IdSolicitacaoCadastroCliente = solicitacao.Id
                    };
                    var addHistorico = await _solicitacaoCadastroRepository.AddHistorico(hst, 4);
                    if (addHistorico)
                    {
                        var bodyEmail = BuildTemplateEmail.Make(_server, "\\templates\\templates-email\\correcao_cadastro.html", null);
                        var enviaEmail = _emailService.Send(solicitacao.Usuario.Email, bodyEmail, "Correção de cadastro");

                        return Json(new { code = 1, msg = "Interação adiciona com sucesso" });
                    }
                    return Json(new { code = 0, msg = "Falha ao salvar interação" });
                }
                return Json(new { code = 0, msg = "Não é possível " });
            }
            return Json(new { code = 0, msg = "Falha ao salvar interação" });
        }

        [Route("finalizar-solicitacao")]
        [PermissionFilter((int)Permissao.Alterar, (int)EnumModuloSistema.Clientes)]
        public async Task<IActionResult> Finalizar(Guid? idSolicitacao)
        {
            if (idSolicitacao == null)
                return RedirectToAction("Detalis", new { id = idSolicitacao });

            var solicitacao = await _solicitacaoCadastroRepository.GetSoliticacao(idSolicitacao);
            if (solicitacao != null)
            {
                var hst = new HstSolicitacao
                {
                    Observacao = "Solicitação Finalizada pelo atendente",
                    IsCliente = false,
                    IdUsuario = Convert.ToInt32(idUsuario),
                    IdSolicitacaoCadastroCliente = solicitacao.Id
                };
                var addHistorico = await _solicitacaoCadastroRepository.AddHistorico(hst, 8);
                if (addHistorico)



                    return RedirectToAction("SolicitacaoCadastroCliente");
            }
            return RedirectToAction("Detalis", new { id = idSolicitacao });
        }
    }
}
