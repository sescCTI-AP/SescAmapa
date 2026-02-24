using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SqlClient;
using PagamentoApi.Models;

namespace PagamentoApi.Repositories
{
    public class SiteRepository
    {
        private readonly IConfiguration _configuration;
        public SiteRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
         }

        public async Task<bool> BaixaInscricaoEvento(string txid)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("SITE")))
                {
                    connection.Open();

                    var inscricao = await connection.QueryAsync<bool>(@"UPDATE InscricaoEvento set Pago = 1 where TxidPix = @txid ", new { txid = txid });
                   
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }



    }
}