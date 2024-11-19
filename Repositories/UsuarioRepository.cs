using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.Admin;
using SiteSesc.Models.ApiClient;
using SiteSesc.Models.Enums;
using SiteSesc.Models.ViewModel;
using SiteSesc.Services;
using System.Configuration;
using System.Data;
using System.Text;

namespace SiteSesc.Repositories
{
    public class UsuarioRepository
    {
        private SiteSescContext _dbContext;
        public readonly HostConfiguration hostConfiguration;
        private readonly ApiClient _apiClient;
        private bool devMode;
        private IWebHostEnvironment env;
        public readonly IConfiguration configuration;


        public UsuarioRepository([FromServices] SiteSescContext context, IConfiguration configuration, ApiClient apiClient, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            devMode = configuration.GetSection("Development")["mode"] == "development";
            hostConfiguration = configuration.GetSection("HostConfig").Get<HostConfiguration>();
            _dbContext = context;
            _apiClient = apiClient;
            this.env = env;
        }


        public async Task<dynamic> Autenticar(UsuarioViewModel usuario)
        {
            try
            {
                if (_apiClient.token == null) await _apiClient.ObtemLoginSite(usuario);
                var contentJson = JsonConvert.SerializeObject(usuario);
                var response = await _apiClient.Cliente.PostAsync($"/v1/login",
                    new StringContent(contentJson, Encoding.UTF8, "application/json"));

                    var successContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var usuarioAutenticado = JsonConvert.DeserializeObject<UserLogin>(successContent);

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

      

            public async Task<List<Usuario>> ObterUsuarioColaboradorPorNome(string nome)
        {
            var usuarios = _dbContext.Usuario
                .Where(u => u.Nome.Contains(nome) && u.IdPerfilUsuario != 2).ToList();
            if (usuarios != null)
            {
                return usuarios;
            }
            return null;
        }

        public async Task<Usuario> GetUsuario(Guid guid)
        {
            var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(g => g.Guid == guid);
            if (usuario == null)
                throw new Exception("Usuário nao encontrado");
            return usuario;
        }

        public async Task<Usuario> GetUsuarioCpf(string cpf)
        {
            var usuario = await _dbContext.Usuario.FirstOrDefaultAsync(g => g.Cpf.Trim() == cpf.Trim());
            if (usuario == null)
                throw new Exception("Usuário nao encontrado");
            return usuario;
        }

        public async Task<Usuario> GetUsuarioById(int? id)
        {
            if (id != null)
            {
                var usuario = await _dbContext.Usuario.AsNoTracking().Include(a => a.adm_usuarioModuloSistema).ThenInclude(a => a.adm_acoesUsuarioModuloSistema).FirstOrDefaultAsync(g => g.Id == id);
                return usuario;
            }
            return null;
        }

        public async Task<adm_acoesUsuarioModuloSistema[]> GetAcoesModulo(int? id)
        {
            if (id != null)
            {
                var acoes = await _dbContext.adm_acoesUsuarioModuloSistema.Where(a => a.IdUsuarioModuloSistema == id).ToArrayAsync();
                return acoes;
            }
            return null;
        }


        public async Task<List<PerfilUsuario>> GetPerfilUsuario()
        {
            return await _dbContext.PerfilUsuario.ToListAsync();
        }

        public List<adm_moduloSistema> GetModulos()
        {
            return _dbContext.adm_moduloSistema.ToList();
        }

        public List<adm_acaoSistema> GetAcoes()
        {
            return _dbContext.adm_acaoSistema.Include(a => a.adm_acoesUsuarioModuloSistema).ToList();
        }

        public async Task<bool> AddAutorizacaoUsuario(SetAutorizacaoViewModel listaAutorizacao)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (listaAutorizacao != null)
                    {
                        var usuario = await ObterUsuarioPorCpf(listaAutorizacao.cpf);
                        if (usuario != null)
                        {
                            //remove, se houver, os dados de outorização cadastrados anteriormente
                            if (usuario.IdPerfilUsuario == (int)PerfilUsuarioEnum.Cliente)
                            {
                                usuario.IdPerfilUsuario = (int)PerfilUsuarioEnum.Funcionario;
                                _dbContext.Entry(usuario).State = EntityState.Modified;
                            }

                            var acoesUsuarioModuloSistema = _dbContext.adm_acoesUsuarioModuloSistema.Where(a => a.adm_usuarioModuloSistema.IdUsuario == usuario.Id).ToList();
                            var modulosUsuario = _dbContext.adm_usuarioModuloSistema.Where(u => u.IdUsuario == usuario.Id).ToList();

                            if (acoesUsuarioModuloSistema != null && acoesUsuarioModuloSistema.Any())
                            {
                                _dbContext.adm_acoesUsuarioModuloSistema.RemoveRange(acoesUsuarioModuloSistema);
                            }
                            if (modulosUsuario != null && modulosUsuario.Any())
                            {
                                _dbContext.adm_usuarioModuloSistema.RemoveRange(modulosUsuario);
                            }
                            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                            //insere os novos dados de autorização
                            foreach (var item in listaAutorizacao.listaAutorizacao)
                            {
                                var usuarioModuloSistema = new adm_usuarioModuloSistema();
                                usuarioModuloSistema.IdUsuario = usuario.Id;
                                usuarioModuloSistema.IdModuloSistema = item.IdModulo;
                                await _dbContext.adm_usuarioModuloSistema.AddAsync(usuarioModuloSistema);
                                await _dbContext.SaveChangesAsync();

                                if (item.Acoes != null && item.Acoes.Any())
                                {
                                    foreach (var idAcaoSistema in item.Acoes)
                                    {
                                        _dbContext.adm_acoesUsuarioModuloSistema.Add(new adm_acoesUsuarioModuloSistema
                                        {
                                            IdAcaoSistema = idAcaoSistema,
                                            IdUsuarioModuloSistema = usuarioModuloSistema.Id
                                        });
                                        await _dbContext.SaveChangesAsync();
                                    }
                                }
                            }
                            transaction.Commit();
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> Ativar(Guid guid)
        {
            try
            {
                var usuario = await GetUsuario(guid);
                usuario!.IsAtivo = true;
                _dbContext.Entry(usuario).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DefinirTelaInicial(int? idUsuario, string rota)
        {
            try
            {
                var telaInicial = _dbContext.TelaInicialAdmin.Where(a => a.IdUsuario == idUsuario).ToList();
                if (telaInicial != null)
                {
                    if (telaInicial.Any())
                    {
                        _dbContext.TelaInicialAdmin.RemoveRange(telaInicial);
                    }
                }
                var tela = new TelaInicialAdmin
                {
                    IdUsuario = (int)idUsuario,
                    Rota = rota
                };
                _dbContext.Add(tela);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Usuario verificaLogin(UsuarioViewModel usuario)
        {
            usuario.Senha = Seguranca.Sha256(usuario.Senha);
            var usuarioLogado = new Usuario();
            if (env.IsDevelopment())
            {
                usuarioLogado = _dbContext.Usuario
                    .Include(u => u.PerfilUsuario)
                    .SingleOrDefault(u => (u.Username.Equals(usuario.Username) || u.Cpf.Equals(usuario.Username)));

            }
            else
            {
                usuarioLogado = _dbContext.Usuario
                .Include(u => u.PerfilUsuario)
                .SingleOrDefault(u => (u.Username.Equals(usuario.Username) || u.Cpf.Equals(usuario.Username)) && u.Senha.Equals(usuario.Senha));
            }

            return usuarioLogado;
        }

        public bool VerificaUsuarioExistente(string cpf, string username, string email)
        {
            if (_dbContext.Usuario.Any(c => c.Cpf.Trim() == cpf.Trim()))
            {
                throw new ArgumentException("CPF já utilizado");
            }
            if (_dbContext.Usuario.Any(c => c.Email.Trim() == email.Trim()))
            {
                throw new ArgumentException("Email já utilizado");
            }
            if (_dbContext.Usuario.Any(c => c.Username.ToLower().Trim() == username.ToLower().Trim()))
            {
                throw new ArgumentException("Usuário já utilizado");
            }
            if (_dbContext.Usuario.Any(c => c.Username.ToLower().Trim() == cpf.Trim()))
            {
                throw new ArgumentException("Usuário já utilizado");
            }
            return true;
        }


        public async Task<List<Usuario>> GetFuncionarios()
        {
            return await _dbContext.Usuario.Where(u => u.IdPerfilUsuario != 4).OrderBy(u => u.Nome).ToListAsync();
        }

        public async Task<Usuario> ObterUsuarioPorCpf(string Cpf)
        {
            Cpf = Cpf.Replace(".", "").Replace("-", "");
            var usuario = await _dbContext.Usuario.AsNoTracking().Include(a => a.adm_usuarioModuloSistema).ThenInclude(a => a.adm_acoesUsuarioModuloSistema).FirstOrDefaultAsync(u => u.Cpf == Cpf);
            if (usuario != null)
            {
                return usuario;
            }
            return null;
        }

        public async Task<List<UsuarioList>> GetUsuarios(string? busca = null)
        {
            try
            {
                using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
                {
                    var sql = @"SELECT u.Id, u.Nome, u.Cpf, u.Email, p.Nome as Perfil, u.IsAtivo, u.DataCadastro  FROM Usuario
                    u join PerfilUsuario p on u.IdPerfilUsuario = p.Id ";

                    if (!string.IsNullOrEmpty(busca))
                    {
                        sql += $" WHERE u.Nome like '%{busca}%' OR u.Cpf = '{busca}'";
                    }
                    sql += $" Order by u.Nome";
                    var cliente = await connection.QueryAsync<UsuarioList>(
                        sql);
                    return cliente.ToList();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro no DAO: " + ex.Message);
                return null;
            }
        }

        public async Task<PaginatedList<UsuarioList>> GetPaginatedRecordsAsync(int pageNumber, int pageSize, string? busca = null)
        {
            try
            {
                List<UsuarioList> query = await GetUsuarios(busca);

                int totalRecords = query.Count();

                var records = query?.Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToList();

                return new PaginatedList<UsuarioList>(records, totalRecords, pageNumber, pageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }


        public async Task<List<Usuario>> GetUsuariosPorFiltro(string filtro)
        {
            return _dbContext.Usuario.Include(u => u.PerfilUsuario).Where(u => u.Nome.ToUpper().Contains(filtro.ToUpper()) || u.Cpf.Trim() == filtro.Trim()).ToList();
        }

        public async Task<List<Usuario>> GetUsuariosPorCpf(string cpf)
        {
            return _dbContext.Usuario.Include(u => u.PerfilUsuario).Where(u => u.Cpf.ToUpper().Contains(cpf.ToUpper())).ToList();
        }

        public async Task<bool> UpdateUsuario(Usuario usuario)
        {
            try
            {
                _dbContext.Entry(usuario).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> RemoveModulos(int perfil, Usuario usuario)
        {
            try
            {
                //remove, se houver, os dados de outorização cadastrados anteriormente
                var acoesUsuarioModuloSistema = _dbContext.adm_acoesUsuarioModuloSistema.Where(a => a.adm_usuarioModuloSistema.IdUsuario == usuario.Id);
                if (acoesUsuarioModuloSistema.Any())
                {
                    _dbContext.adm_acoesUsuarioModuloSistema.RemoveRange(acoesUsuarioModuloSistema);
                    await _dbContext.SaveChangesAsync();
                }
                if (usuario.adm_usuarioModuloSistema != null && usuario.adm_usuarioModuloSistema.Any())
                {
                    _dbContext.adm_usuarioModuloSistema.RemoveRange(usuario.adm_usuarioModuloSistema);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public async Task<List<int>> GetModulosByUsuario(int idUsuario)
        {
            return await _dbContext.adm_usuarioModuloSistema.Where(u => u.IdUsuario == idUsuario).Select(a => a.IdModuloSistema).Distinct().ToListAsync();
        }


        public async Task<List<int>> GetAcoesUsuarioModulo(int idUsuario, int idModuloSistema)
        {
            try
            {
                int idUsuarioModuloSitema = 0;
                int perfilUsuario = _dbContext.Usuario.Find(idUsuario).IdPerfilUsuario;
                if (perfilUsuario == (int)PerfilUsuarioEnum.SysAdmin)
                {
                    return AddAllPermission();
                }

                if (perfilUsuario == (int)PerfilUsuarioEnum.Administrador)
                {
                    return AdmPermission();
                }

                var usuarioModuloSitema = _dbContext.adm_usuarioModuloSistema.FirstOrDefault(u => u.IdUsuario == idUsuario && u.IdModuloSistema == idModuloSistema);
                if (usuarioModuloSitema == null)
                {
                    return null;
                }
                if (usuarioModuloSitema != null)
                {
                    idUsuarioModuloSitema = usuarioModuloSitema.Id;
                }

                var acoesUsuarioModuloSistema = await _dbContext.adm_acoesUsuarioModuloSistema.Where(a => a.IdUsuarioModuloSistema == idUsuarioModuloSitema).ToListAsync();

                var idsAcoes = acoesUsuarioModuloSistema != null ? acoesUsuarioModuloSistema.Select(i => i.IdAcaoSistema).ToList() : null;

                return idsAcoes;

            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }
        }

        public List<int> AddAllPermission()
        {
            int[] permissions = { 1, 2, 3, 4,5 };
            List<int> allPermissions = new List<int>();
            allPermissions.AddRange(permissions);
            return allPermissions;
        }


        public List<int> AdmPermission()
        {
            int[] permissions = { 1, 2, 3, 4 };
            List<int> gPermissions = new List<int>();
            gPermissions.AddRange(permissions);
            return gPermissions;
        }
        public List<int> ReadPermission()
        {
            int[] permissions = { 1 };
            List<int> readPermissions = new List<int>();
            readPermissions.AddRange(permissions);
            return readPermissions;
        }

        public bool UsuarioExists(Guid id)
        {
            return _dbContext.Usuario.Any(e => e.Guid == id);
        }

    }
}