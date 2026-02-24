using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Site;

namespace PagamentoApi.Repositories
{
    public class LoginRepository
    {
        private readonly IConfiguration configuration;
        public LoginRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SescTO_App getApp(string app)
        {
            try
            {
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    var connectionString = configuration.GetConnectionString("BDPROD");

                    connection.Open();

                    var usuario = connection.QueryFirstOrDefault<SescTO_App>(@"SELECT * FROM SESCTO_APP WHERE APP = @APP", new { APP = app });
   
                    connection.Close();
                    return usuario;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public UsuarioSite GetUsuarioSite(string cpf)
        {
            //tira - e . do cpf.
            cpf = cpf.Replace("-", "").Replace(".", "");

            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                UsuarioSite usuario = connection.QueryFirstOrDefault<UsuarioSite>(@"SELECT * FROM Usuario WHERE Cpf = @cpf", new { cpf });
                return usuario;
            }
        }

        public UsuarioSite GetUsuarioSiteEmail(string email)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                UsuarioSite usuario = connection.QueryFirstOrDefault<UsuarioSite>(@"SELECT * FROM Usuario WHERE Email = @email", new { email });
                return usuario;
            }
        }

        public async Task<string> GenerateRefreshToken(string cpf)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                var refreshToken = Guid.NewGuid().ToString();
                var sql = @"UPDATE Usuario SET RefreshToken = @refreshToken WHERE Cpf = @cpf";
                var affectedRows = await connection.ExecuteAsync(sql, new { refreshToken, cpf });
                if (affectedRows <= 0)
                {
                    return null;
                }

                return refreshToken.Trim();
            }

        }
    }
}