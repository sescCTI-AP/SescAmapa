using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.ModelPartialView;
using SiteSesc.Models.ViewModel;
using SiteSesc.Repositories;
using SiteSesc.Services;
using System.Security.Claims;
using System.IO;
using System.Text.RegularExpressions;

namespace SiteSesc.Controllers
{


    [Route("atividade")]
    public class AtividadeController : Controller
    {
        private readonly AtividadeOnLineReposotory _atividadeRepository;
        private readonly ClienteRepository _clienteRepository;
        private readonly DefaultRepository _defaultRepository;
        private readonly AreaRepository _areaRepository;

        public AtividadeController(AtividadeOnLineReposotory atividadeOnLineReposotory, ClienteRepository clienteRepository, DefaultRepository defaultRepository, AreaRepository areaRepository)
        {
            _atividadeRepository = atividadeOnLineReposotory;
            _clienteRepository = clienteRepository;
            _defaultRepository = defaultRepository;
            _areaRepository = areaRepository;
        }


        [Route("{id?}/{cduop?}")]
        public async Task<IActionResult> Atividades(int pageNumber = 1, int pageSize = 12, int? idArea = null, int? idUop = null)
        {
            var paginatedList = await _atividadeRepository.GetPaginatedRecords(pageNumber, pageSize, idArea, idUop);
            var viewModel = new PaginatedListViewModel<CursoItem>
            {
                Items = paginatedList,
                PageIndex = paginatedList.PageIndex,
                TotalPages = paginatedList.TotalPages,
                HasPreviousPage = paginatedList.HasPreviousPage,
                HasNextPage = paginatedList.HasNextPage
            };
            if (idArea.HasValue)
            {
                ViewBag.AreaSelecionada = await _areaRepository.GetArea(idArea);
                viewModel.IdArea = idArea;
            }
            if (idUop.HasValue)
            {
                ViewBag.UopSelecionada = await _defaultRepository.GetUopById((int)idUop);
                viewModel.IdUop = idUop;
            }
            ViewBag.idUop = new SelectList(await _defaultRepository.ObtemUnidadesAtividades(), "Id", "Nome", idUop);
            ViewBag.Areas = await _areaRepository.GetAreasAtivas();
            return View(viewModel);
        }

        [Route("detalhes/{cdelement}")]
        public async Task<IActionResult> Details(string cdelement)
        {
            var turma = Util.CdelementToTurma(cdelement);
            var atividadeApi = await _atividadeRepository.ObterAtividade(turma);
            var atividadeSite = await _atividadeRepository.GetAtividadeSite(turma);
            var formasPgto = await _atividadeRepository.ObterFormasPgtoCdelement(cdelement);
            var horarios = await _atividadeRepository.ObterHorariosCdelement(cdelement);
            var valorAtividade = await _atividadeRepository.ObterValores(turma, formasPgto);

            var atv = AtividadeViewModel.ToAtividade(atividadeApi, atividadeSite, horarios, valorAtividade);
            var atividade = (AtividadeViewModel)atv;

            var isLogado = ((ClaimsIdentity)User.Identity).FindFirst("CPF") != null ? true : false;
            ViewBag.Logado = isLogado;


            // Obtém a URL atual
            var request = HttpContext.Request;
            var currentUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            // Passa a URL para a view
            ViewBag.CurrentUrl = currentUrl;
            //ViewBag.msgInscrito = msgInscrito == null ? "" : msgInscrito;
            ViewBag.AtvTituloUrl = atividade.NomeExibicao;
            ViewBag.AtvTextoUrl = Regex.Replace(atividade.Descricao, "<.*?>", string.Empty);
            ViewBag.AtvCaminhoVirtual = atividade.Imagem;

            

            if (isLogado)
            {
                var cpf = ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
                var clientes = await _clienteRepository.ObterClienteDependentesPorCpf(cpf);
                var valores = (List<AtividadeValor>)valorAtividade;
                foreach (var cliente in clientes)
                {

                    if (valores.Any(a => a.cdcategori == cliente.CDCATEGORI))
                        cliente.VLPARCELA = valores.FirstOrDefault(a => a.cdcategori == cliente.CDCATEGORI).vlparcela;
                    else
                        ViewBag.SemValor = "Sem valor configurado. Por favor entrar em contato com a central de relacionamento";

                }
                if (clientes.Count() > 1)
                {
                    ViewBag.Clientes = clientes.ToList();
                }
                var usuTitul = clientes.FirstOrDefault(c => c.NUCPF.Trim() == cpf.Trim());

                ViewBag.Usuario = usuTitul;
                ViewBag.Vencido = usuTitul.DTVENCTO.Date < DateTime.Now.Date ? "1" : "0";
            }
            return View(atividade);
        }

        [Authorize]
        [Route("finalizar-compra/{cdelement}/{cpfcliente}")]
        public async Task<IActionResult> FinalizarCompra(string cdelement, string cpfCliente)
        {
            // Variáveis para o responsável (pode ser necessário em outro contexto)
            string cpfResponsavel = "";
            string nomeResponsavel = "";
            string rgResponsavel = "";

            var turma = Util.CdelementToTurma(cdelement);
            var cpfInscricao = !string.IsNullOrEmpty(cpfCliente) ? cpfCliente : ((ClaimsIdentity)User.Identity).FindFirst("CPF").Value;
            string termo = "";

            // 1. Obter o cliente
            var cliente = await _clienteRepository.ObterClientePorCpf(cpfInscricao);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            // 2. Calcular a idade
            var idade = DateTime.Today.Year - cliente.Dtnascimen.Year;
            if (cliente.Dtnascimen.Date > DateTime.Today.AddYears(-idade)) idade--; // Corrige caso ainda não tenha feito aniversário

            // 3. Obter a atividade e verificar os limites de idade
            var atividade = await _atividadeRepository.ObterAtividade(turma);

            if (atividade != null)
            {
                int idadeMin = (int)atividade.idademin; // Supondo que idademin seja um tipo numérico.
                int idadeMax = (int)atividade.idademax;

                if (idade < atividade.idademin || idade > atividade.idademax)
                {
                    return PartialView("_Validation", new MsgValidation
                    {
                        Message = $"A idade do cliente deve estar entre {idadeMin} e {idadeMax} anos."
                    });
                }
            }

            // Continuação do processamento
            var atividadeSite = await _atividadeRepository.GetAtividadeSite(turma);
            var templateTermo = await _atividadeRepository.GetTemplateTermo(cdelement);
            var formasPgto = (List<FormasPgto>)await _atividadeRepository.ObterFormasPgtoCdelement(atividadeSite.cdelement);
            var horarios = await _atividadeRepository.ObterHorariosCdelement(atividadeSite.cdelement);
            var valores = await _atividadeRepository.ObterValores(turma, formasPgto);

            // Validação de categorias
            if (!valores.Any(a => a.cdcategori == cliente.Cdcategori))
            {
                return PartialView("_Validation", new MsgValidation 
                { Message = "Valores não definidos para essa categoria. Por favor contate a Central de Relacionamento para mais informações." });
            }

            // Processamento do termo
            if (templateTermo != null)
            {
                var responsavel = await _clienteRepository.ObterResponsavel(cliente.Cduop, cliente.Sqmatric);
                if (responsavel.Count() > 0)
                    cliente = ClienteCentral.SetResponsavel(cliente, responsavel.FirstOrDefault());

                var endereco = new Models.ApiPagamento.Endereco();
                if (cliente.Enderecos.Count() > 0)
                    endereco = cliente.Enderecos.FirstOrDefault();

                var tabelaValores = await _atividadeRepository.DemoMensalidades(atividade, cliente, formasPgto.LastOrDefault());

                termo = await Util.SubstituirVariaveis<ClienteCentral>(templateTermo.termo, cliente);
                termo = await Util.SubstituirVariaveis<AtividadeApi>(termo, atividade);
                termo = await Util.SubstituirVariaveis<Models.ApiPagamento.Endereco>(termo, endereco);

                var cob = "";
                if (tabelaValores.Any())
                {
                    foreach (var item in tabelaValores)
                        cob += $"{item.DTVENCTO.ToShortDateString()} &nbsp; &nbsp; &nbsp; {item.VLCOBRADO.ToString("C")} </br>";
                }
                termo = termo.Replace("[[ tableValores ]]", cob);
            }

            var InscricaoViewModel = new FinalizarInscricao
            {
                Cliente = cliente,
                AtividadeApi = atividade,
                AtividadeSite = atividadeSite,
                TemplateTermo = termo,
                Horarios = horarios,
                FormasPgto = formasPgto.FirstOrDefault(),
                Valor = valores.FirstOrDefault(a => a.cdcategori == cliente.Cdcategori && a.vlparcela > 0).vlparcela,
            };

            return PartialView(InscricaoViewModel);
        }


        [Authorize]
        [Route("inscrever")]
        public async Task<IActionResult> Inscrever(Inscricao inscricao)
        {
                var inscricaoAtividade = new Inscricao
            {
                CDPROGRAMA = Convert.ToInt32(inscricao.CDPROGRAMA),
                CDCONFIG = Convert.ToInt32(inscricao.CDCONFIG),
                SQOCORRENC = Convert.ToInt32(inscricao.SQOCORRENC),
                CDUOP = Convert.ToInt32(inscricao.CDUOP),
                SQMATRIC = Convert.ToInt32(inscricao.SQMATRIC),
                CDFORMATO = Convert.ToInt32(inscricao.CDFORMATO)
            };

            var cliente = await _clienteRepository.GetClienteCentralAtendimento(inscricao.CDUOP, inscricao.SQMATRIC);

            if (cliente != null)
            {
                if (cliente.DTVENCTO.Date < DateTime.Now.Date)
                {
                    return RedirectToAction("Details", new
                    {
                        cdelement = inscricaoAtividade.cdelement
                    });
                }

                if (inscricaoAtividade != null)
                {
                    var inscricaoSubmit = await _atividadeRepository.Inscricao(inscricaoAtividade);
                    if (inscricaoSubmit != "")
                    {
                        return RedirectToAction("Details", new
                        {
                            cdelement = inscricaoAtividade.cdelement,
                            msgInscrito = "1",
                        });
                    }
                }
                return RedirectToAction("CobrancasPorAtividade", "Cobranca", new
                {                    
                    area = "Cliente",
                    cdelement = inscricaoAtividade.cdelement,
                    cpf = cliente.NUCPF,
                    inscricao = true
                });
                //TODO: ROUTE PARA A PAGINA DE MENSALIDADES
              /*   return RedirectToRoute(new
                 {                     
                     controller = "cobrancas",
                     action = "atividade",
                     cdelement = inscricaoAtividade.cdelement,
                     cpf = cliente.NUCPF,
                     inscricao = true
                 });*/

            }
            return View();
        }
    }


    
}
