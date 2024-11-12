using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteSesc.Repositories
{
    public class EmpresaRepository
    {
        private IWebHostEnvironment _env;
        private HostConfiguration _host;
        private readonly SiteSescContext _dbContext;
        private bool devMode;
        private readonly IConfiguration _configuration;
        private DefaultRepository _defaultRepository;
        public EmpresaRepository(SiteSescContext dbContext, IWebHostEnvironment env, HostConfiguration host, IConfiguration configuration, DefaultRepository defaultRepository)
        {
            _dbContext = dbContext;
            _env = env;
            _host = host;
            devMode = configuration.GetSection("Development")["mode"] == "development";
            _defaultRepository = defaultRepository;

        }

        //<--------GET EMPRESA-------->
        public async Task<Empresa> GetEmpresa(string? cnpj)
        {
            try
            {
                var consultaEmpresa = _dbContext.Empresa.SingleOrDefault(c => c.Cnpj == cnpj);
                if (consultaEmpresa == null)
                {
                    return null;
                }
                return consultaEmpresa;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //<--------ADICIONAR EMPRESA-------->
        public async Task<dynamic> AddEmpresa(Empresa empresa, string telefone)
        {
            if (empresa != null)
            {
                try
                {
                    var template = @"\wwwroot\templates\templatesemail\cadastro_empresa.html";
                    List<string> destino = new List<string> { "compras@sescto.com.br" };
                    //var destino = "wescley188@gmail.com";
                    var assunto = "Novo Cadastro de Empresa - Portal SESC";
                    Dictionary<string, string> contentFileReplace = new Dictionary<string, string>()
                    {
                        { "[[fantasia]]", empresa.NomeFantasia },
                        { "[[razao]]", empresa.RazaoSocial},
                        {"[[cnpj]]", empresa.Cnpj},
                        {"[[ramo]]", empresa.Ramo},
                        {"[[endereco]]", empresa.Endereco.Logradouro},
                        { "[[numero]]", empresa.Endereco.Numero.ToString()},
                        {"[[cep]]", empresa.Endereco.Cep},
                        {"[[bairro]]", empresa.Endereco.Bairro},
                        {"[[cidade]]", empresa.Endereco.Cidade},
                        {"[[estado]]", empresa.Endereco.Estado},
                        {"[[telefone]]", telefone},
                        {"[[email]]", empresa.Email}
                    };

                    await _dbContext.Empresa.AddAsync(empresa);
                    AddTelefone(empresa, telefone);
                    //var emailEnviado = generalRepository.EnviarEmail(template, destino, assunto, contentFileReplace);
                    //if (emailEnviado)
                    //{
                    //    await _dbContext.SaveChangesAsync();
                    //    return true;
                    //}
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        //<--------ADICIONAR TELEFONE-------->
        public void AddTelefone(Empresa empresa, string telefone)
        {
            if (!string.IsNullOrEmpty(telefone))
            {
                var telefoneNovo = new Telefone();
                telefoneNovo.Numero = telefone;

                empresa.TelefoneEmpresa.Add(new TelefoneEmpresa
                {
                    Empresa = empresa,
                    Telefone = telefoneNovo
                });
            }
        }

        public dynamic GetRetornaEmpresa(string cnpj)
        {
            try
            {
                string cnpjSemFormatacao = Regex.Replace(cnpj, @"[^0-9]+", "");
                var client = new RestClient($"https://api.cnpja.com/office/{cnpjSemFormatacao}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "c684f6b0-88b2-4ca5-b359-27f8df1510bd-d4423745-6e34-4b96-b51b-a99d234dbc5c");
                IRestResponse response = client.Execute(request);
                //EmpresaCnpjJa empresaJson = JsonConvert.DeserializeObject<EmpresaCnpjJa>(response.Content);
                var empresaJson = Util.GetDadosEmpresaByCnpj(cnpj).Result;
                return empresaJson;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Usuario verificaLogin(string cnpj, string senha)
        {
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            senha = Seguranca.Sha256(senha);
            var usuarioLogado = new Usuario();
            if (devMode)
            {
                return usuarioLogado = _dbContext.Usuario.FirstOrDefault(u => u.Cpf == cnpj && u.IsEmpresa);
            }
            return _dbContext.Usuario.SingleOrDefault(u => u.Cpf.Equals(cnpj) && u.Senha.Equals(senha) && u.IsEmpresa);
        }



        public Usuario GetUsuarioEmpresa(string cnpj)
        {
            return _dbContext.Usuario.SingleOrDefault(a => a.Cpf == cnpj && a.IsEmpresa);
        }

        public Empresa GetDadosEmpresa (string cnpj)
        {
            return _dbContext.Empresa.SingleOrDefault(a => a.Cnpj == cnpj);
        }

        public void Telefone(int idcli, string primario, string secundario)
        {
            var telefone = new Telefone()
            {
                Numero = primario,
                Observacao = "Telefone Principal",
            };

            _dbContext.Telefone.Add(telefone);
            _dbContext.SaveChanges();
        }
        public void Protocolo(Guid idSolicitacao, int usuEmpId, int cliId)
        {
            var protocolo = new HstSolicitacao()
            {
                IdUsuario = usuEmpId,
                IdSolicitacaoCadastroCliente = idSolicitacao,
                DataCadastro = DateTime.Now,
                IsCliente = true //Tipo Trabalhador do Comércio
            };

            _dbContext.HstSolicitacao.Add(protocolo);
            _dbContext.SaveChanges();
        }
    }
}
