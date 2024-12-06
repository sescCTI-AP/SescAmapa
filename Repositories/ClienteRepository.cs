using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.ApiClient;
using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.ApiPagamento.Relatorios;
using SiteSesc.Models.DB2;
using SiteSesc.Models.ViewModel;
using SiteSesc.Services;
using System;
using System.Data;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SiteSesc.Repositories
{
    public class ClienteRepository
    {
        private SiteSescContext _dbContext;
        public readonly HostConfiguration hostConfiguration;
        private readonly ApiClient _apiClient;
        private bool devMode;

        public ClienteRepository([FromServices] SiteSescContext context, IConfiguration configuration, ApiClient apiClient)
        {
            devMode = configuration.GetSection("Development")["mode"] == "development";
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _dbContext = context;
            _apiClient = apiClient;
        }



        public async Task<decimal> ObterSaldoCartao(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cartao/cpf/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var saldo = JsonConvert.DeserializeObject<decimal>(successContent);
                    return saldo;
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



        public async Task<dynamic> ObtemCredencial(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/cartao/{cpf}");
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

        public async Task<List<HSTMOVCART>> ObterMovimentacaoCartao(int? cartao)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cartao/movimentacoes/{cartao}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<HSTMOVCART>>(successContent);
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
        public async Task<int?> ObtemTipoCategoria(int id)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/tipocategoria/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    return Convert.ToInt32(successContent);
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

        public async Task<string> ObtemCategoria(int id)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/categoria/{id}");
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

        public async Task<CLIENTELA> GetClienteCentralAtendimento(int cdUop, int sqMatric)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/{cdUop}/{sqMatric}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cliente = JsonConvert.DeserializeObject<CLIENTELA>(successContent);
                    return cliente;
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

        public async Task<List<Responsavel>> ObterResponsavel(int cdUop, int sqMatric)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/usuario/responsavel/{cdUop}/{sqMatric}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cliente = JsonConvert.DeserializeObject<List<Responsavel>>(successContent);
                    return cliente;
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

        public async Task<ClienteCentral> ObterClientePorCpf(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/usuario/cliente/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var code = response.StatusCode;
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cliente = JsonConvert.DeserializeObject<ClienteCentral>(successContent);
                    return cliente;
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

        public async Task<ClienteCentral> GetTitular(int cdUop, int sqMatric)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/{cdUop}/{sqMatric}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var clienteDependente = JsonConvert.DeserializeObject<CLIENTELA>(successContent);
                    if (clienteDependente.CDUOTITUL != null)
                    {
                        var titular = await GetClienteCentralAtendimento((int)clienteDependente.CDUOTITUL, (int)clienteDependente.SQTITULMAT);
                        var clienteDb2 = await ObterClientePorCpf(titular.NUCPF); //CLIENTE NO DB2
                        return clienteDb2;
                    }
                    return null;
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

        public async Task<List<ClientelaViewModel>> ObterClienteDependentesPorCpf(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/cpf/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var code = response.StatusCode;
                    var successContent = await response.Content.ReadAsStringAsync();
                    var cliente = JsonConvert.DeserializeObject<List<ClientelaViewModel>>(successContent);
                    return cliente.ToList();
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

        public async Task<RelatorioIndicadores> GetIndicadoresClientes(int mes, int ano)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/indicadores/{mes}/{ano}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<RelatorioIndicadores>(successContent);
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

        public async Task<List<MunicipioSelect>> GetMunicipios()
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/municipios");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<List<MunicipioSelect>>(successContent);
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

        public async Task<bool> HasCliente(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"/v1/cliente/hascliente/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<bool>(successContent);
                    return retorno;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        public async Task<dynamic> AddClienteDb2(ClienteAdd cliente)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var contentJson = JsonConvert.SerializeObject(cliente);
                var response = await _apiClient.Cliente.PostAsync($"/v1/cliente/AddClienteDb2",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    var retorno = JsonConvert.DeserializeObject<ClienteAdd>(successContent);
                    return retorno;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorContent);
                return errorContent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }

        public async Task<PaginatedList<SolicitacaoCadastroCliente>> GetPaginatedRecordsAsync(int pageNumber, int pageSize, int? IdStatus = null)
        {
            try
            {
                IQueryable<SolicitacaoCadastroCliente> query = _dbContext.SolicitacaoCadastroCliente.Include(a => a.Usuario).Include(a => a.Status);
                if (IdStatus.HasValue)                
                    query = query.Where(a => a.IdStatus == IdStatus.Value);
                
                int totalRecords = query.Count();

                var records = query?.Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();

                return new PaginatedList<SolicitacaoCadastroCliente>(records, totalRecords, pageNumber, pageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public async Task<List<string>> GetMatriculasTitularDependentes(List<string> listaCpf)
        {
            try
            {
                if (listaCpf.Any())
                {
                    var listaAtividades = new List<MatriculasCentral>();
                    foreach (var item in listaCpf)
                    {
                        var matriculas = await GetMatriculas(item);
                        if (matriculas != null) listaAtividades.AddRange(matriculas);
                    }
                    if (listaAtividades.Any())
                    {
                        return listaAtividades.Select(a => a.cdelement).ToList();
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<dynamic> gerarToken(string cpf)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginApp();
                var response = await _apiClient.Cliente.GetAsync($"v1/cliente/infoCliente/{cpf}");
                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    // var usuarioAutenticado = JsonConvert.DeserializeObject<UserLogin>(successContent);

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

    }
}