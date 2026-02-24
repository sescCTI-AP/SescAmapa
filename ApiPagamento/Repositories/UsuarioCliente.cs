using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;

namespace PagamentoApi.Repositories
{
    public class UsuarioCliente
    {
        private readonly IConfiguration configuration;
        public UsuarioCliente(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public CLIENTELA getUsuario(string cpf)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var usuario = connection.QueryFirstOrDefault<CLIENTELA>(@"SELECT * FROM CLIENTELA WHERE NUCPF = @CPF", new { CPF = cpf });
                return usuario;
            }
        }
    }
}