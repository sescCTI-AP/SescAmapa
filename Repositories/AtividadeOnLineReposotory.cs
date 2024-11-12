using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.ApiPagamento.Relatorios;
using SiteSesc.Models.Atividade;
using SiteSesc.Models.Avaliacao;
using SiteSesc.Models.DB2;
using SiteSesc.Models.ModelPartialView;
using SiteSesc.Models.Relatorio;
using SiteSesc.Models.Rematricula;
using SiteSesc.Models.SiteViewModels;
using SiteSesc.Models.ViewModel;
using SiteSesc.Services;
using System.Linq;
using System.Text;
using static SiteSesc.Models.Status;

namespace SiteSesc.Repositories
{
    public class AtividadeOnLineReposotory
    {
        private readonly SiteSescContext _dbContext;
        public readonly IConfiguration configuration;
        private readonly ApiClient _apiClient;
        private readonly ClienteRepository _clienteRepository;

        public AtividadeOnLineReposotory(SiteSescContext dbContext, IConfiguration configuration, ApiClient apiClient, ClienteRepository clienteRepository)
        {
            _dbContext = dbContext;
            this.configuration = configuration;
            _apiClient = apiClient;
            _clienteRepository = clienteRepository;
        }

        public async Task<PaginatedList<CursoItem>> GetPaginatedRecords(int pageNumber, int pageSize, int? idArea = null, int? idUop = null)
        {
            var listCursos = new List<CursoItem>();
            IQueryable<AtividadeOnLine> query = _dbContext.AtividadeOnLine.Include(a => a.Arquivo).Include(a => a.UnidadeOperacional).Include(a => a.SubArea).ThenInclude(a => a.Area).Include(a => a.Usuario).Where(a => a.IsAtivo);

            if (idUop.HasValue)
            {
                query = query.Where(a => a.IdUnidadeOperacional == idUop.Value);
            }

            if (idArea.HasValue)
            {
                query = query.Where(a => a.SubArea.IdArea == idArea.Value);
            }

            int totalRecords = await query.CountAsync();
            var records = await query.Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            if (records.Count() > 0)
            {
                var listaAtividades = (List<AtividadeOnLine>)records;
                foreach (var atividade in listaAtividades)
                {
                    var atividadeApi = await ObterAtividade(atividade.turma);
                    if (atividadeApi != null)
                    {
                        var formasPgto = await ObterFormasPgtoCdelement(atividade.cdelement);
                        var horarios = await ObterHorariosCdelement(atividade.cdelement);
                        var valorAtividade = await ObterValores(atividade.turma, formasPgto);
                        var valor = valorAtividade.Count() > 0 ? valorAtividade.Min(va => va.vlparcela).ToString() : null;
                        listCursos.Add(new CursoItem((int)atividade.SubArea.IdArea, atividade.cdelement, atividade.Arquivo.CaminhoVirtualFormatado(), atividade.NomeExibicao, atividade.SubArea.Area.Nome, atividade.UnidadeOperacional.Nome, valor, atividade.Descricao, null));
                    }
                }
            }

            return new PaginatedList<CursoItem>(listCursos, totalRecords, pageNumber, pageSize);
        }

        public async Task<dynamic> Get(int? id = null)
        {
            if (id != null)
            {
                return await _dbContext.AtividadeOnLine.Include(a => a.Arquivo).Include(a => a.SubArea).ThenInclude(a => a.Area).Include(a => a.UnidadeOperacional).Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);
            }
            return await _dbContext.AtividadeOnLine.Include(a => a.Arquivo).Include(a => a.Usuario).Include(a => a.UnidadeOperacional).Include(a => a.SubArea).ThenInclude(a => a.Area).Where(a => a.DataDesativacao == null).ToListAsync();
        }

        public async Task<PaginatedList<AtividadeOnLine>> GetPaginatedRecordsAsync(int pageNumber, int pageSize, int? idUop = null)
        {
            IQueryable<AtividadeOnLine> query = _dbContext.AtividadeOnLine.Include(a => a.UnidadeOperacional).Include(a => a.Usuario).OrderByDescending(a=>a.DataCadastro);

            if (idUop.HasValue)
            {
                query = query.Where(a => a.IdUnidadeOperacional == idUop.Value);
            }

            int totalRecords = await query.CountAsync();
            var records = await query.Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            return new PaginatedList<AtividadeOnLine>(records, totalRecords, pageNumber, pageSize);
        }

        public async Task<List<MatriculasCentral>> GetMatriculas(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<MatriculasCentral>>(successContent);
                    return retorno;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        public async Task<List<AvaliacaoAtividadeCliente>> ObtemAvaliacoes(string cdelement)
        {
            try
            {
                int cdprograma = Convert.ToInt32(cdelement.Substring(0, 8));
                int cdconfig = Convert.ToInt32(cdelement.Substring(8, 8));
                int sqocorrenc = Convert.ToInt32(cdelement.Substring(16, 8));
                var avaliacaoAtividade = await _dbContext.AvaliacaoAtividadeCliente.Where(m =>
                m.cdprograma == cdprograma &&
                m.cdconfig == cdconfig &&
                m.sqocorrec == sqocorrenc).OrderByDescending(d => d.DataCadastro).ToListAsync();

                return avaliacaoAtividade;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<AvaliacaoAtividadeCliente> AvaliacaoAtividadeCliente(AvaliacaoAtividadeCliente avaliacaoAtividadeCliente, int? idCliente)
        {
            try
            {
                if (idCliente != null)
                {
                    avaliacaoAtividadeCliente.DataCadastro = DateTime.Now;
                    avaliacaoAtividadeCliente.IdUsuario = (int)idCliente;
                    await _dbContext.AvaliacaoAtividadeCliente.AddAsync(avaliacaoAtividadeCliente);
                    await _dbContext.SaveChangesAsync();

                    return avaliacaoAtividadeCliente;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<List<AvaliacaoAtividadeCliente>> ObtemAvaliacoes(string cdelement, int idCliente)
        {
            try
            {
                int cdprograma = Convert.ToInt32(cdelement.Substring(0, 8));
                int cdconfig = Convert.ToInt32(cdelement.Substring(8, 8));
                int sqocorrenc = Convert.ToInt32(cdelement.Substring(16, 8));
                var avaliacaoAtividade = await _dbContext.AvaliacaoAtividadeCliente.Where(m =>
                m.cdprograma == cdprograma &&
                m.cdconfig == cdconfig &&
                m.sqocorrec == sqocorrenc &&
                m.IdUsuario == idCliente
                ).OrderByDescending(d => d.DataCadastro).ToListAsync();

                return avaliacaoAtividade;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }



        public async Task<dynamic> GetAtivas(int? id, int? cduop = null, int? qtd = null)
        {
            int qtdAtiv = 8;
            if (qtd == null)
                qtdAtiv = 99999;
            if (id == null && cduop == null)
            {
                return await _dbContext.AtividadeOnLine
                    .Include(a => a.Arquivo)
                    .Include(a => a.Usuario)
                    .Include(a => a.UnidadeOperacional)
                    .Include(a => a.SubArea)
                    .ThenInclude(a => a.Area)
                    .Where(a => a.DataDesativacao == null && a.IsAtivo)
                    .Take(qtdAtiv).ToListAsync();
            }
            else if (id != null && cduop == null)
            {
                return await _dbContext.AtividadeOnLine
                .Include(a => a.Arquivo)
                .Include(a => a.Usuario)
                .Include(a => a.UnidadeOperacional)
                .Include(a => a.SubArea)
                .ThenInclude(a => a.Area)
                .Where(a => a.DataDesativacao == null && a.IsAtivo && a.SubArea.IdArea == id)
                .Take(qtdAtiv).ToListAsync();
            }
            else if (id != null && cduop != null)
            {
                return await _dbContext.AtividadeOnLine
                .Include(a => a.Arquivo)
                .Include(a => a.Usuario)
                .Include(a => a.UnidadeOperacional)
                .Include(a => a.SubArea)
                .ThenInclude(a => a.Area)
                .Where(a => a.DataDesativacao == null && a.IsAtivo && a.SubArea.IdArea == id && a.UnidadeOperacional.Cduop == cduop)
                .Take(qtdAtiv).ToListAsync();
            }

            return await _dbContext.AtividadeOnLine
                .Include(a => a.Arquivo)
                .Include(a => a.Usuario)
                .Include(a => a.UnidadeOperacional)
                .Include(a => a.SubArea)
                .ThenInclude(a => a.Area)
                .Where(a => a.DataDesativacao == null && a.IsAtivo && a.UnidadeOperacional.Cduop == cduop)
                .Take(qtdAtiv).ToListAsync();
        }

        public async Task<dynamic> GetAtivasUop(int? id)
        {
            if (id == null)
            {
                return await _dbContext.AtividadeOnLine
                    .Include(a => a.Arquivo)
                    .Include(a => a.Usuario)
                    .Include(a => a.UnidadeOperacional)
                    .Include(a => a.SubArea)
                    .ThenInclude(a => a.Area)
                    .Where(a => a.DataDesativacao == null && a.IsAtivo)
                    .Take(8).ToListAsync();
            }
            return await _dbContext.AtividadeOnLine
                .Include(a => a.Arquivo)
                .Include(a => a.Usuario)
                .Include(a => a.UnidadeOperacional)
                .Include(a => a.SubArea)
                .ThenInclude(a => a.Area)
                .Where(a => a.DataDesativacao == null && a.IsAtivo && a.UnidadeOperacional.Cduop == id)
                .Take(8).ToListAsync();
        }

        public async Task<dynamic> GetAtividadeSite(Turma turma)
        {
            if (turma == null)
            {
                return null;
            }
            return await _dbContext.AtividadeOnLine
                .Include(a => a.Arquivo)
                .Include(a => a.SubArea)
                .ThenInclude(a => a.Area)
                .Include(a => a.UnidadeOperacional)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Cdprograma == turma.CDPROGRAMA && a.Cdconfig == turma.CDCONFIG && a.Sqocorrenc == turma.SQOCORRENC && a.IsAtivo);
        }

        public async Task<AtividadeOnLine> GetAtividade(Turma turma)
        {
            try
            {
                var atividade = await _dbContext.AtividadeOnLine
                .Include(a => a.Arquivo)
                .Include(a => a.UnidadeOperacional)
                .ThenInclude(a => a.Endereco)
                .Include(a => a.SubArea)
                .ThenInclude(a => a.Area)
                .FirstAsync(a => a.Cdprograma == turma.CDPROGRAMA && a.Cdconfig == turma.CDCONFIG && a.Sqocorrenc == turma.SQOCORRENC && a.IsAtivo == true && a.DataDesativacao == null);

                if (atividade != null)
                {
                    return atividade;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<bool> AtvExist(string cdelement)
        {

            var cdprograma = Convert.ToInt32(cdelement.Substring(0, 8));
            var cdconfig = Convert.ToInt32(cdelement.Substring(8, 8));
            var sqocorrenc = Convert.ToInt32(cdelement.Substring(16, 8));

            return await _dbContext.AtividadeOnLine.AnyAsync(a => a.Cdprograma == cdprograma && a.Cdconfig == cdconfig && a.Sqocorrenc == sqocorrenc && a.IsAtivo);
        }

        public async Task<AtividadeOnLine> AvividadePorCdelment(string cdelement)
        {

            var cdprograma = Convert.ToInt32(cdelement.Substring(0, 8));
            var cdconfig = Convert.ToInt32(cdelement.Substring(8, 8));
            var sqocorrenc = Convert.ToInt32(cdelement.Substring(16, 8));

            return await _dbContext.AtividadeOnLine.FirstOrDefaultAsync(a => a.Cdprograma == cdprograma && a.Cdconfig == cdconfig && a.Sqocorrenc == sqocorrenc && a.IsAtivo);
        }

        public async Task<dynamic> Editar(AtividadeOnLine atividade)
        {
            try
            {
                if (atividade != null)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            // Buscar a atividade original para preservar o IdUsuario
                            var atividadeExistente = await _dbContext.AtividadeOnLine
                                .FirstOrDefaultAsync(a => a.Id == atividade.Id);

                            if (atividadeExistente != null)
                            {
                                // Preservar o IdUsuario original
                                atividade.IdUsuario = atividadeExistente.IdUsuario;

                                // Atualizar os outros campos manualmente
                                atividadeExistente.NomeExibicao = atividadeExistente.NomeExibicao;
                                atividadeExistente.Descricao = atividade.Descricao;
                                atividadeExistente.DataInicio = atividade.DataInicio;
                                atividadeExistente.DataFim = atividade.DataFim;
                                atividadeExistente.IdSubArea = atividade.IdSubArea;
                                atividadeExistente.IsAtivo = atividade.IsAtivo;
                                atividadeExistente.IdArquivo = atividade.IdArquivo;
                                atividadeExistente.IsGratuito = atividade.IsGratuito;
                                atividadeExistente.IsOnline = atividade.IsOnline;
                                atividadeExistente.DescontoPontualidade = atividade.DescontoPontualidade;

                                // Salvar as mudanças
                                await _dbContext.SaveChangesAsync();
                                await transaction.CommitAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(ex.Message);
                            throw new Exception(ex.Message);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<dynamic> Salvar(AtividadeOnLine atividade)
        {
            try
            {
                if (atividade != null)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            if (atividade.Id == 0)
                            {
                                _dbContext.Add(atividade);
                            }
                            else
                            {
                                _dbContext.Entry(atividade).State = EntityState.Modified;
                            }
                            await _dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(ex.Message);
                            throw new Exception(ex.Message);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> AlteraStatus(int id, bool isAtivo, int? idUsuario = null)
        {
            try
            {
                var atividade = await Get(id);
                atividade.IdUsuario = idUsuario != null ? idUsuario : atividade.IdUsuario;
                if (!isAtivo)
                    atividade.DataDesativacao = DateTime.Now;
                atividade.IsAtivo = isAtivo;
                _dbContext.Entry(atividade).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<AtividadeOnLine>> GetAtividadesPendentes()
        {
            try
            {
                var atividades = await _dbContext.AtividadeOnLine.Include(a => a.Arquivo).Include(a => a.SubArea).Include(a => a.UnidadeOperacional).Where(a => !a.IsAtivo && a.DataDesativacao == null).ToListAsync();
                if (atividades != null)
                {
                    return atividades;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<AtividadeApi>> ObterAtividadesAno(int uop, int? ano = null)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/turmas/{uop}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var atividades = JsonConvert.DeserializeObject<List<AtividadeApi>>(successContent);
                    if (ano != null)
                    {
                        atividades = atividades.Where(a => a.dtfimocorr?.Year > ano).ToList();
                    }
                    return atividades;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<AtividadeApi> ObterAtividade(Turma turma)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(turma);
                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/turma",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<AtividadeApi>(successContent);
                    if (retorno != null)
                    {
                        return retorno;
                    }
                    return null;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<AtividadeApi>> ObterAtividadesDb2CdElement(List<string> listaCdElement)
        {
            try
            {
                List<AtividadeApi> listaAtividadeApi = new List<AtividadeApi>();
                if (listaCdElement.Any())
                {
                    foreach (var item in listaCdElement)
                    {
                        var cdPrograma = Convert.ToInt32(item.Substring(0, 8));
                        var cdconfig = Convert.ToInt32(item.Substring(8, 8));
                        var sqocorrenc = Convert.ToInt32(item.Substring(16, 8));
                        var turma = Util.MakeTurma(cdPrograma, cdconfig, sqocorrenc);
                        var atividadeApi = await ObterAtividade(turma);
                        if (atividadeApi != null)
                        {
                            listaAtividadeApi.Add(atividadeApi);
                        }
                    }
                    return listaAtividadeApi;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<List<InscritosViewModel>> Inscritos(Turma turma)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(turma);
                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/usuarios",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var inscritos = JsonConvert.DeserializeObject<List<InscritosViewModel>>(successContent);
                    return inscritos;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<int> Inadimplentes(string cdelement)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(cdelement);
                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/inadimplentes",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var inscritos = JsonConvert.DeserializeObject<int>(successContent);
                    return inscritos;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public async Task<AtividadeValor> ObterValorPorCategoria(Turma turma, int categoria, List<FormasPgto> formasPagamento, int? codformato)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(turma);
                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/valores",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<AtividadeValor>>(successContent);
                    if (retorno != null)
                    {
                        if (codformato != null)
                        {
                            var valor = retorno.FirstOrDefault(v => v.cdcategori == categoria && v.cdformato == codformato);
                            return valor;
                        }
                        else
                        {
                            var cdformato = new List<int>();
                            //var pgtoVista = new List<int>();
                            cdformato = formasPagamento.Where(p => p.tppgto == 0).Select(p => p.cdformato).ToList(); //Verifica se entre as formas de pgto exista o tipo "MENSAL" (tppgto == 0)
                            if (!cdformato.Any())
                            {
                                cdformato = formasPagamento.Where(p => p.nmformato.Contains("VISTA")).Select(p => p.cdformato).ToList(); //Verifica se entre as formas de pgto exista o tipo "À VISTA" (tppgto == 2)
                            }

                            if (categoria == 25 || categoria == 22)
                            {
                                categoria = 1;
                            }
                            var valor = retorno.FirstOrDefault(v => v.cdcategori == categoria /*&& v.vlparcela > 0*/ && cdformato.Contains(v.cdformato));
                            return valor;
                        }

                    }
                    return null;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<AtividadeValor>> ObterValores(Turma turma, List<FormasPgto> formasPagamento)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(turma);
                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/valores",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<AtividadeValor>>(successContent);

                    if (retorno != null)
                    {

                        //int[] categorias = new int[] { 1, 6 }; // DEFINE DE QUAIS CATEGORIAS PEGA OS VALORES

                        var pagamentoMensal = formasPagamento.Where(p => p.nmformato.Contains("MENS") || p.nmformato.Contains("VISTA")).Select(p => p.cdformato);
                        //var pagamentoMensal = formasPagamento.Where(p => p.tppgto == 0 && p.nmformato.Contains("MENS")).Select(p => p.cdformato);

                        if (pagamentoMensal.Count() == 0)
                        {
                            //TODO: PENSAR EM OTIMIZAR A BUSCA DE ISENÇÃO
                            pagamentoMensal = formasPagamento.Where(t => !t.nmformato.Contains("ISEN") || !t.nmformato.Contains("GRATU")).Select(p => p.cdformato);
                        }
                        return retorno.Where(v => /*categorias.Contains(v.cdcategori) && */v.vlparcela > 0 && pagamentoMensal.Contains(v.cdformato)).ToList();
                    }
                    return null;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<COBRANCA>> DemoMensalidades(AtividadeApi atividade, ClienteCentral cliente, FormasPgto formasPgto)
        {
            try
            {

                var inscricao = new Inscricao
                {
                    CDPROGRAMA = atividade.cdprograma,
                    CDCONFIG = atividade.cdconfig,
                    SQOCORRENC = atividade.sqocorrenc,
                    CDUOP = cliente.Cduop,
                    SQMATRIC = cliente.Sqmatric,
                    CDFORMATO = formasPgto != null ? formasPgto.cdformato : 100545
                };


                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(inscricao);

                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/mensalidades", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<COBRANCA>>(successContent);

                    return retorno;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<AtividadeApi>> ObterAtividadesSelect(int uop, int? ano = null)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/turmas-select/{uop}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var atividades = JsonConvert.DeserializeObject<List<AtividadeApi>>(successContent);
                    if (ano != null)
                    {
                        atividades = atividades.Where(a => a.dtfimocorr?.Year > ano).ToList();
                    }
                    return atividades;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public async Task<List<FormasPgto>> ObterFormasPgtoCdelement(string cdelement)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/formas-pagamento-cdelement/{cdelement}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<FormasPgto>>(successContent);
                    return result;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<Horario>> ObterHorariosCdelement(string cdelement)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/horarios-cdelement/{cdelement}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<Horario>>(successContent);
                    return result;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Termo> GetTemplateTermo(string cdelement)
        {
            try
            {
                var cdpgrograma = cdelement.Substring(0, 8);


                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/termo/{cdpgrograma}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var termo = JsonConvert.DeserializeObject<Termo>(successContent);
                    return termo;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<REALIZACAO>> GetModalidades()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/realizacao");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var registros = JsonConvert.DeserializeObject<List<REALIZACAO>>(successContent);
                    return registros;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<SUBMODAL>> GetSubmodalidades()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/submodalidade");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var registros = JsonConvert.DeserializeObject<List<SUBMODAL>>(successContent);
                    return registros;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<PROGRAMAOCORRENCIA>> GetAtividadesAtivas()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/ativas");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var registros = JsonConvert.DeserializeObject<List<PROGRAMAOCORRENCIA>>(successContent);
                    return registros;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<PROGRAMAOCORRENCIA>> GetAtividadesRematricula()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/rematricula");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var registros = JsonConvert.DeserializeObject<List<PROGRAMAOCORRENCIA>>(successContent);
                    return registros;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }




        public async Task<dynamic> Inscricao(Inscricao inscricao)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(inscricao);

                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/inscrever", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    return successContent;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<InscritosPorUnidade>> ObterInscritosPorUop(int? uop, int? ano = null)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/turmas/inscritos/{uop}/{ano}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var atividades = JsonConvert.DeserializeObject<List<InscritosPorUnidade>>(successContent);

                    //atividades = atividades.Where(a => a.aamoda.Equals(ano.ToString())).ToList();
                    var anoAnterior = DateTime.Now.Year - 1;
                    var anoAtual = DateTime.Now.Year;

                    atividades = atividades.Where(a => a.aamoda == anoAnterior.ToString() || a.aamoda == anoAtual.ToString()).ToList();
                    //atividades = atividades.Where(a => a.dtfimocorr > DateTime.Now).ToList();
                    return atividades;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<AtividadeOnLine> GetAtividadeId(int? id)
        {
            try
            {
                var atividade = _dbContext.AtividadeOnLine
                    .Include(a => a.SubArea)
                    .Include(a => a.Usuario)
                    .Include(a => a.UnidadeOperacional)
                    .Include(a => a.Arquivo)
                    .SingleOrDefault(a => a.Id == id && a.DataDesativacao == null);
                return atividade;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<AtividadeOnLine>> GetAll(bool todosOsAnos)
        {
            try
            {
                List<AtividadeOnLine> atividades = new List<AtividadeOnLine>();
                if (todosOsAnos)
                {
                    atividades = await _dbContext.AtividadeOnLine
                                     .Include(a => a.Arquivo)
                                     .Include(a => a.SubArea)
                                     .Include(a => a.Usuario)
                                     .Include(a => a.UnidadeOperacional).Where(a => a.DataDesativacao == null)
                                     .ToListAsync();
                }
                else
                {
                    atividades = await _dbContext.AtividadeOnLine
                                     .Include(a => a.Arquivo)
                                     .Include(a => a.SubArea)
                                     .Include(a => a.Usuario)
                                     .Include(a => a.UnidadeOperacional).Where(a => a.Ano >= DateTime.Now.Year && a.IsAtivo && a.DataDesativacao == null)
                                     .ToListAsync();
                }

                if (atividades != null)
                {
                    return atividades;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            return null;
        }

        public async Task<List<AtividadeOnLine>> ObterAtividadesAtivas()
        {
            try
            {
                var atividades = await _dbContext.AtividadeOnLine
                    .Include(a => a.Arquivo)
                    .Include(a => a.SubArea)
                    .ThenInclude(a => a.Area)
                    .Include(a => a.Usuario)
                    .Include(a => a.UnidadeOperacional).Where(a => a.IsAtivo == true && a.DataDesativacao == null)
                    .ToListAsync();
                if (atividades != null)
                {
                    return atividades;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public async Task<List<MOTCANCEL>> ObtemMotivosCancelamentos()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/atividade/motivos-cancelamentos");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var atividades = JsonConvert.DeserializeObject<List<MOTCANCEL>>(successContent);
                    return atividades;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<HSTCANCELAMENTO>> ListaCancelamentos(FiltroCancelamento filtro)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(filtro);

                var response = await _apiClient.Cliente.PostAsync($"/v1/atividade/relatorio/cancelamentos", new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<HSTCANCELAMENTO>>(successContent);

                    return retorno;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        public async Task<List<TurmaRematriculaViewModel>> ProcessaTurmaRematricula(List<TurmaRematriculaTurmaPermitida> listaTurmas, List<UnidadeOperacional> listaUop)
        {
            if (listaTurmas.Any())
            {
                var listTurmas = new List<TurmaRematriculaViewModel>();

                foreach (var item in listaTurmas)
                {
                    var turma1 = CdElementToTurma(item.CdElement);
                    var turma2 = CdElementToTurma(item.CdElementPermitido);
                    //Turma atv;
                    //if (int.TryParse(item.CdElementAtvAssociada, out int resultado))
                    //    atv = CdElementToTurma(item.CdElementAtvAssociada);
                    //else
                    //    atv = CdElementToTurma(item.CdElement);
                    var t1 = await ObterAtividade(turma1);
                    var t2 = await ObterAtividade(turma2);
                    //var t3 = await ObterAtividade(atv);
                    var unidade = listaUop.FirstOrDefault(a => a.Id == item.IdUop);

                    var turmaViewModel = new TurmaRematriculaViewModel
                    {
                        Id = item.Id,
                        Unidade = unidade!.Nome,
                        TurmaAnterior = t1.dsusuario,
                        TurmaPermitida = t2.dsusuario,
                        cdelementTurmaPermitida = t2.cdelement,
                        cdelementAtvAssociada = "",
                        Material = ""
                    };
                    listTurmas.Add(turmaViewModel);
                }
                return listTurmas;
            }
            return null;
        }

        public Turma CdElementToTurma(string cdelement)
        {
            var cdprograma = Convert.ToInt32(cdelement.Substring(0, 8));
            var cdconfig = Convert.ToInt32(cdelement.Substring(8, 8));
            var sqocorrenc = Convert.ToInt32(cdelement.Substring(16, 8));
            var turma = new Turma
            {
                CDPROGRAMA = cdprograma,
                CDCONFIG = cdconfig,
                SQOCORRENC = sqocorrenc
            };
            return turma;
        }
    }
}
