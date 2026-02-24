using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Tef;

namespace PagamentoApi.Repositories
{
    public class TotemRepository
    {
        public readonly IConfiguration configuration;

        public TotemRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Obtem configuração de totem por ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<ConfigTotem> ObtemTotem(string ip)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = " SELECT * FROM SESCTO_CONFIGTOTEM WHERE IPTOTEM = @ip";
                var totem = await connection.QueryFirstAsync<ConfigTotem>(sql, new { ip = ip} );
                return totem;
            }
        }

        /// <summary>
        /// Obtem configuração de totem por ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<ConfigTotem> ObtemTotemCodigo(string codigo)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @$"SELECT * FROM SESCTO_CONFIGTOTEM WHERE CODTOTEM = @codigo";
                var totem = await connection.QueryFirstAsync<ConfigTotem>(sql, new {codigo = codigo});
                return totem;
            }
        }


        /// <summary>
        /// Obtem sequencia valida de cupom para o totem especificado
        /// </summary>
        /// <returns></returns>
        public async Task<int> ObtemCupom()
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @$"SELECT COALESCE(MAX(SQCUPOM), 0) + 1 AS PROXIMO_CUPOM FROM SESCTO_TRANSACAOTOTEM";
                var cupom = await connection.QueryFirstAsync<int>(sql);
                return cupom;
            }
        }

        /// <summary>
        /// Obtem transacoes por cliente
        /// </summary>
        /// <returns></returns>
        public async Task<List<TransacaoTotem>> ObtemTransacoesPorCliente(string cpf)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = @$"SELECT * FROM SESCTO_TRANSACAOTOTEM WHERE CPF = @cpf ORDER BY ID DESC";
                var cupom = await connection.QueryAsync<TransacaoTotem>(sql, new { cpf = cpf});
                return cupom.ToList();
            }
        }

        /// <summary>
        /// Registra transacao do totem
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RegistraTransacao(TransacaoTotem transacao)
        {
            transacao.SQCUPOM = await ObtemCupom();

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sql = "INSERT INTO DB2DBA.SESCTO_TRANSACAOTOTEM " +
                    "(CDELEMENT, SQCOBRANCA, IDCLASSE, NOMEPAGADOR, CARDNUMBER, IDTRANSACAO, TIPOPAGAMENTO, VALOR, CDPESSOA, DATATRANSACAO, " +
                    "COMPROVANTECLIENTE, COMPROVANTELOJA, SQDEPRET, CAIXA, CANCELADO, SQCUPOM, CODTOTEM, CUPOM, CPF) " +
                    "VALUES" +
                    "(@CDELEMENT, @SQCOBRANCA, @IDCLASSE, @NOMEPAGADOR, @CARDNUMBER, @IDTRANSACAO, @TIPOPAGAMENTO, @VALOR, @CDPESSOA, " +
                    "@DATATRANSACAO, @COMPROVANTECLIENTE, @COMPROVANTELOJA, @SQDEPRET, @CAIXA, @CANCELADO, @SQCUPOM, @CODTOTEM, @CUPOM, @CPF)";
                var affectedRows = await connection.ExecuteAsync(sql, transacao);
                if (affectedRows <= 0)
                {
                    return false;
                }
                else
                    return true;
            }
        }

        public async Task<bool> TransacaoJaIniciada(TransacaoTotem transacao)
        {

            try
            {
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    connection.Open();

                    var sql = @"SELECT count(CUPOM) FROM SESCTO_TRANSACAOTOTEM  WHERE CUPOM = @CUPOM ";
                    var result = await connection.QueryFirstOrDefaultAsync<bool>(sql, transacao);

                    return result;
                }

            }
            catch (System.Exception e)
            {
                return false;                

            }

        }

    }
}