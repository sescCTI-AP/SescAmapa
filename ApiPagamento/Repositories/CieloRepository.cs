using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;

namespace PagamentoApi.Repositories
{
    public class CieloRepository
    {

        private readonly IConfiguration configuration;

        public CieloRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /// <summary>
        /// Obtem o caixa PDV para o usuario especificados no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<List<PagamentoCielo>> ObterTransacoes(int caixa)
        {
            using(var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();

                var transacoes = await connection.QueryAsync<PagamentoCielo>(
                    @"SELECT * FROM SESCTO_CIELO WHERE CANCELADO != 1 AND CAIXA = @CAIXA ORDER BY DTOPERACAO DESC",
                    new
                    {
                        caixa
                    });
                return transacoes.AsList();
            }
        }

    }
}