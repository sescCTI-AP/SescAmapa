using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Settings;

namespace PagamentoApi.Repositories
{
    public class CaixaRepository
    {
        private readonly IConfiguration configuration;

        public readonly CaixaSettings caixaConfiguration;

        public CaixaRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            caixaConfiguration = configuration.GetSection("CaixaSettings").Get<CaixaSettings>();
        }

        /// <summary>
        /// Obtem o caixa do usuário especificado no arquivo de configuração com os pagamentos desse caixa
        /// </summary>
        /// <returns></returns>
        public CACAIXA GetCaixaPorUsuario(string usuario)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var caixaDictionary = new Dictionary<string, CACAIXA>();
                var caixa = connection.Query<CACAIXA, PAGAMENTOS, CACAIXA>(
                        @"SELECT * FROM CACAIXA AS C LEFT JOIN PAGAMENTOS AS P ON C.CDPESSOA = P.CDPESSOA AND C.SQCAIXA = P.SQCAIXA WHERE C.CDPESSOA = @usuario ORDER BY C.SQCAIXA DESC",
                        (caixa, pagamento) =>
                        {
                            CACAIXA caixaEntry;

                            if (!caixaDictionary.TryGetValue($"{caixa.IDUSUARIO}-{caixa.SQCAIXA}", out caixaEntry))
                            {
                                caixaEntry = caixa;
                                caixaEntry.PAGAMENTOS = new List<PAGAMENTOS>();
                                caixaDictionary.Add($"{caixa.IDUSUARIO}-{caixa.SQCAIXA}", caixaEntry);
                            }

                            caixaEntry.PAGAMENTOS.Add(pagamento);
                            return caixaEntry;

                        },
                        new
                        {
                            usuario = caixaConfiguration.CdPessoa

                        }, splitOn: "IDCLASSE");
                return caixa.FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtem o caixa PDV para o usuario especificados no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<CACAIXA> ObterCaixa(int sqcaixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var caixa = await connection.QueryAsync<CACAIXA>(
                    @"SELECT * FROM CACAIXA WHERE CDPESSOA = @usuario AND SQCAIXA = @sqcaixa ORDER BY SQCAIXA DESC",
                    new
                    {
                        usuario = caixaConfiguration.CdPessoa,
                        sqcaixa
                    });
                return caixa.FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtem o caixa PDV para o usuario especificados no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<List<CaixaPartial>> ObterListaCaixas(string ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                ano += "%";
                connection.Open();
                var caixas = await connection.QueryAsync<CaixaPartial>(
                    @"SELECT SQCAIXA, NUFECHAMEN FROM CACAIXA WHERE CDPESSOA = @usuario AND NUFECHAMEN LIKE @ano ORDER BY SQCAIXA DESC",
                    new
                    {
                        usuario = caixaConfiguration.CdPessoa,
                        ano
                    });
                return caixas.AsList();
            }
        }
        /// <summary>
        /// Obtem o caixa PDV aberto para o usuario especificados no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<CACAIXA> ObterCaixaAberto(bool caixaTef = false)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                //var sql = @"SELECT * FROM CACAIXA WHERE CDPESSOA = @usuario AND STCAIXA = 0 ORDER BY SQCAIXA DESC";

                var cdpessoa = caixaConfiguration.CdPessoa;
                if (caixaTef == true)
                    cdpessoa = caixaConfiguration.CdPessoaTef;
                
                var caixaDictionary = new Dictionary<string, CACAIXA>();
                var caixa = await connection.QuerySingleOrDefaultAsync<CACAIXA>(
                    @"SELECT * FROM CACAIXA WHERE CDPESSOA = @usuario AND STCAIXA = 0 ORDER BY SQCAIXA DESC",
                    new
                    {
                        usuario = cdpessoa
                    });
                return caixa;
            }
        }
        /// <summary>
        /// Obtem ultimo caixadepret PDV para o usuario especificados no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<CXDEPRETPDV> ObterCaixaDePret(CACAIXA cacaixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var caixaDictionary = new Dictionary<string, CXDEPRETPDV>();
                var caixa = await connection.QueryFirstOrDefaultAsync<CXDEPRETPDV>(
                    @"SELECT * FROM CXDEPRETPDV WHERE CDPESSOA = @usuario AND SQCAIXA = @caixa ORDER BY SQDEPRET DESC",
                    new
                    {
                        usuario = cacaixa.CDPESSOA,
                        caixa = cacaixa.SQCAIXA
                    });
                return caixa;
                //STDEPRET 0 = Normal
                //STDEPRET 1 = Cancelado
            }
        }

        /// <summary>
        /// Obtem o saldo total do caixa de pret do PDV
        /// </summary>
        /// <returns>decimal</returns>
        public async Task<decimal> ObterSaldoCxPretPdv(CACAIXA caixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var saldo = await connection.QueryFirstOrDefaultAsync<decimal?>(
                    @"SELECT SUM(VLDEPRET) SALDO FROM CXDEPRETPDV WHERE CDPESSOA = @cdpessoa AND SQCAIXA = @sqcaixa",
                    new
                    {
                        cdpessoa = caixa.CDPESSOA,
                        sqcaixa = caixa.SQCAIXA
                    });
                return saldo != null ? saldo.Value : 0;
            }

        }

        /// <summary>
        /// Abre o caixa PDV para o usuario e estacao especificados no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> AbrirCaixa(bool caixaTef = false)
        {
            var cdpessoa = caixaConfiguration.CdPessoa;
            if (caixaTef == true)
                cdpessoa = caixaConfiguration.CdPessoaTef;

            var result = new ReturnMessage();
            var sql = "INSERT INTO CACAIXA (CDPESSOA, SQCAIXA, CDLOCVENDA, CDPDV, CDUOP, DTABERTURA, NUFECHAMEN, HRABERTURA, NMESTACAO, STCAIXA, SMFIELDATU, VLSALDOANT, VLSALDOATU)" +
                "VALUES(@CDPESSOA, @SQCAIXA, @CDLOCVENDA, @CDPDV, @CDUOP, @DTABERTURA, @NUFECHAMEN, @HRABERTURA, @NMESTACAO, @STCAIXA, @SMFIELDATU, @VLSALDOANT, @VLSALDOATU)";
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sqlUltimoCaixa = @$"SELECT * FROM CACAIXA WHERE CDPESSOA = {cdpessoa} ORDER BY SQCAIXA DESC";
                var ultimoCaixa = await connection.QueryFirstOrDefaultAsync<CACAIXA>(sqlUltimoCaixa);
                if (ultimoCaixa == null)
                {
                    ultimoCaixa = new CACAIXA
                    {
                        NUFECHAMEN = Convert.ToInt32(DateTime.Now.Year.ToString() + "0000"),
                        SQCAIXA = 0,
                        VLSALDOATU = 0,
                        STCAIXA = 1,
                        DTFECHAMEN = DateTime.Now
                    };
                }
                if (ultimoCaixa.STCAIXA != 0 && ultimoCaixa.DTFECHAMEN != null)
                {
                    int ano = Convert.ToInt32(ultimoCaixa.NUFECHAMEN.ToString().Substring(0, 4));
                    int nufechamento = Convert.ToInt32(DateTime.Now.Year.ToString() + "0001");
                    if (ano == DateTime.Now.Year)
                    {
                        nufechamento = ultimoCaixa.NUFECHAMEN + 1;
                    }
                    var caixa = new CACAIXA
                    {
                        CDPESSOA = cdpessoa,
                        SQCAIXA = ultimoCaixa.SQCAIXA + 1,
                        CDLOCVENDA = caixaConfiguration.CdLocalVenda,
                        CDPDV = caixaConfiguration.CdPdv,
                        CDUOP = caixaConfiguration.CdUop,
                        DTABERTURA = DateTime.Now,
                        NUFECHAMEN = nufechamento, //TODO: Verificar se ano atual
                        HRABERTURA = DateTime.Now.TimeOfDay,
                        NMESTACAO = caixaConfiguration.NmEstacao,
                        STCAIXA = 0,
                        SMFIELDATU = 0.0,
                        VLSALDOANT = ultimoCaixa.VLSALDOATU.Value,
                        VLSALDOATU = ultimoCaixa.VLSALDOATU
                    };
                    var affectedRows = await connection.ExecuteAsync(sql, caixa);
                    if (affectedRows <= 0)
                    {
                        result.Codigo = 1;
                        result.Message = "Ocorreu um erro ao abrir o caixa";
                        return result;
                    }
                    else
                        return caixa;

                }
                result.Codigo = 2;
                result.Message = "Já existe um caixa aberto para esse usuário";
                return result;
            }

        }

        /// <summary>
        /// Fecha o caixa PDV do usuário especificado no arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnMessage> FecharCaixa(bool caixaTef = false)
        {
            var result = new ReturnMessage();
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();

                var cdpessoa = caixaConfiguration.CdPessoa;
                if (caixaTef == true)
                    cdpessoa = caixaConfiguration.CdPessoaTef;

                var sql = @$"SELECT CDPESSOA, SQCAIXA, DTFECHAMEN, STCAIXA , HRFECHAMEN, LGFECHAMEN FROM CACAIXA WHERE CDPESSOA = {cdpessoa} AND STCAIXA = 0 ORDER BY SQCAIXA DESC";

                var caixa = await connection.QueryFirstOrDefaultAsync<CACAIXA>(
                    sql);
                if (caixa.STCAIXA == 0 && caixa.DTFECHAMEN == null)
                {
                    var retiradas = await this.ObterTotalLancamentosCaixa(caixa);
                    var saldoCaixa = caixa.VLSALDOATU != null ? caixa.VLSALDOATU.Value : 0;
                    var saldoPdv = await this.ObterSaldoCxPretPdv(caixa);
                    var pagamentos = await this.ObterPagamentos(caixa);
                    //var vendasRefeicao = await this.ObterVendasRefeicao(caixa);

                    var saldoTotal = pagamentos + saldoCaixa + saldoPdv  - retiradas;

                    if (saldoTotal > 0)
                    {
                        result.Codigo = 1;
                        result.Message = "O caixa possui saldo que deve ser retirado antes do fechamento.";
                        return result;
                    }
                    caixa.DTFECHAMEN = DateTime.Now.Date;
                    caixa.HRFECHAMEN = DateTime.Now.TimeOfDay;
                    caixa.STCAIXA = 1;
                    caixa.LGFECHAMEN = caixa.CDPESSOA.ToString();
                    caixa.SMFIELDATU = 312.0;
                    var sql2 = "UPDATE CACAIXA SET DTFECHAMEN = @DTFECHAMEN, STCAIXA = 1, HRFECHAMEN = @HRFECHAMEN, LGFECHAMEN = @LGFECHAMEN WHERE CDPESSOA = @CDPESSOA AND SQCAIXA = @SQCAIXA";
                    var affectedRows = await connection.ExecuteAsync(sql2, caixa);
                    if (affectedRows <= 0)
                    {
                        result.Codigo = 1;
                        result.Message = "Ocorreu um erro ao fechar o caixa";
                    }
                    else
                    {
                        result.Codigo = 0;
                        result.Message = "Caixa fechado com sucesso";
                    }
                    return result;
                }
                result.Codigo = 2;
                result.Message = "Já existe um caixa aberto para esse usuário";
                return result;
            }

        }

        private async Task<decimal> ObterPagamentos(CACAIXA caixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT SUM(VLRECEBIDO) " +
                    " FROM PAGAMENTOS WHERE CDPESSOA = @cdPessoa AND SQCAIXA = @sqCaixa";
                var caixaPagamentos = await connection.QueryFirstOrDefaultAsync<Decimal?>(sql, new { caixa.CDPESSOA, caixa.SQCAIXA });
                return caixaPagamentos != null ? caixaPagamentos.Value : 0;
            }
        }
        private async Task<decimal> ObterVendasRefeicao(CACAIXA caixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT  SMVENDA FROM CAIXARESUM WHERE CDPESSOA = @cdPessoa AND SQCAIXA = @sqCaixa";
                var caixaVendasRefeicao = await connection.QueryFirstOrDefaultAsync<Decimal?>(sql, new { caixa.CDPESSOA, caixa.SQCAIXA });
                return caixaVendasRefeicao != null ? caixaVendasRefeicao.Value : 0;
            }
        }

        /// <summary>
        /// Efetua a retirada do saldo total do caixa para fechamento
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> EfetuarRetiradaDoCaixa(bool caixaTef = false)
        {
            var caixa = new CACAIXA();
            var cdpessoa = 0;
            if (caixaTef == true)
            {
                caixa = await this.ObterCaixaAberto(true);
                cdpessoa = caixaConfiguration.CdPessoaTef;
            }
            else
            {
                caixa = await this.ObterCaixaAberto();
                cdpessoa = caixaConfiguration.CdPessoa;
            }

            if (caixa == null)
            {
                return "Não existe um caixa aberto.";
            }
            else if (caixa.VLSALDOATU.Equals(0))
            {
                return "Não existe saldo a ser retirado do caixa";
            }
            else
            {
                var ultimoLancamento = await this.ObterUltimoLancamento(cdpessoa, caixa.SQCAIXA);
                var caixaLancamento = new CAIXALANCA();
                caixaLancamento.SQCAIXA = caixa.SQCAIXA;
                caixaLancamento.SQLANCAMEN = ((ultimoLancamento == null) ? 1 : (caixaLancamento.SQLANCAMEN + 1));
                caixaLancamento.CDPESSOA = cdpessoa;
                caixaLancamento.TPLANCAMEN = 0;
                caixaLancamento.IDUSRLANCA = cdpessoa.ToString();
                caixaLancamento.DTLANCAMEN = DateTime.Now.Date;
                caixaLancamento.HRLANCAMEN = DateTime.Now.TimeOfDay;
                caixaLancamento.DSLANCAMEN = "Retirada do Caixa para Fechamento";
                var valor = caixa.VLSALDOATU != null ? caixa.VLSALDOATU.Value : 0; //Adiciona saldo atual do caixa
                valor = valor + caixa.VLSALDOANT; //Adiciona saldo anterior do caixa
                var valorCxPDV = await this.ObterSaldoCxPretPdv(caixa); // Obtem saldo do caixa PDV
                var valorPagamentos = await this.ObterPagamentos(caixa); // Obtem saldo dos pagamentos recebidos

                // var valorVendaRefeicao = await this.ObterVendasRefeicao(caixa); // Obtem o saldo das vendas das Refeicoes recebidas

                valor = valor + valorCxPDV + valorPagamentos; // + valorVendaRefeicao; // Soma saldo caixa PDV ao caixa

                caixaLancamento.VLLANCAMEN = valor;
                caixaLancamento.STLANCAMEN = 1;
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var sql = "INSERT INTO CAIXALANCA (SQCAIXA, SQLANCAMEN, TPLANCAMEN, IDUSRLANCA, DTLANCAMEN, HRLANCAMEN, DSLANCAMEN, VLLANCAMEN, " +
                                " STLANCAMEN, CDPESSOA) VALUES " +
                                "(@SQCAIXA, @SQLANCAMEN, @TPLANCAMEN, @IDUSRLANCA, @DTLANCAMEN, @HRLANCAMEN, @DSLANCAMEN, @VLLANCAMEN, @STLANCAMEN, @CDPESSOA)";
                            var result = await connection.ExecuteAsync(sql, caixaLancamento, transaction);
                            caixa.VLSALDOATU = new decimal(0);
                            sql = "UPDATE CACAIXA SET VLSALDOATU = @VLSALDOATU WHERE CDPESSOA = @CDPESSOA AND SQCAIXA = @SQCAIXA";
                            result = await connection.ExecuteAsync(sql, caixa, transaction);
                            await transaction.CommitAsync();
                            return caixaLancamento;
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(e);
                            return $"Erro ao fazer a retirada do caixa: {e.Message}";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtem o ultimo lançamento de caixa
        /// </summary>
        /// <returns></returns>
        private async Task<CAIXALANCA> ObterUltimoLancamento(int cdPessoa, int sqCaixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT IDUSUARIO, SQCAIXA, SQLANCAMEN, TPLANCAMEN, IDUSRLANCA, DTLANCAMEN, HRLANCAMEN, DSLANCAMEN, VLLANCAMEN, STLANCAMEN," +
                    "DSSTATUS, CDPESSOA FROM CAIXALANCA WHERE CDPESSOA = @cdPessoa AND SQCAIXA = @sqCaixa ORDER BY SQCAIXA";
                var caixaLancamento = await connection.QueryFirstOrDefaultAsync<CAIXALANCA>(sql, new { cdPessoa, sqCaixa });
                return caixaLancamento;
            }
        }

        /// <summary>
        /// Obtem os lançamentos do caixa
        /// </summary>
        /// <returns></returns>
        private async Task<List<CAIXALANCA>> ObterLancamentosCaixa(CACAIXA caixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT IDUSUARIO, SQCAIXA, SQLANCAMEN, TPLANCAMEN, IDUSRLANCA, DTLANCAMEN, HRLANCAMEN, DSLANCAMEN, VLLANCAMEN, STLANCAMEN," +
                    "DSSTATUS, CDPESSOA FROM CAIXALANCA WHERE CDPESSOA = @cdPessoa AND SQCAIXA = @sqCaixa";
                var caixaLancamento = await connection.QueryAsync<CAIXALANCA>(sql, new { caixa.CDPESSOA, caixa.SQCAIXA });
                return caixaLancamento.ToList();
            }
        }

        /// <summary>
        /// Obtem os lançamentos do caixa
        /// </summary>
        /// <returns></returns>
        private async Task<Decimal> ObterTotalLancamentosCaixa(CACAIXA caixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT SUM(VLLANCAMEN) " +
                    " FROM CAIXALANCA WHERE CDPESSOA = @cdPessoa AND SQCAIXA = @sqCaixa";
                var caixaLancamento = await connection.QueryFirstOrDefaultAsync<Decimal?>(sql, new { caixa.CDPESSOA, caixa.SQCAIXA });
                return caixaLancamento != null ? caixaLancamento.Value : 0;
            }
        }

        public async Task<List<RecargaPaga>> ObterLancamentosPdv(CACAIXA caixa)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = " SELECT CX.SQDEPRET AS Sequencia, TRIM(CF.MATFORMAT) AS Habilitacao, TRIM(CLI.NMCLIENTE) AS Cliente, CX.DTDEPRET AS Data, " +
                    "CX.VLDEPRET AS Valor, TRIM(M.DSMOEDAPGT) AS Moeda, CI.TID " +
                    "FROM CXDEPRETPDV AS CX " +
                    "INNER JOIN CLIENTELA AS CLI ON CX.NUMCARTAO = CLI.NUMCARTAO " +
                    "INNER JOIN CLIFORMAT AS CF ON CLI.CDUOP = CF.CDUOP AND CLI.SQMATRIC = CF.SQMATRIC " +
                    "INNER JOIN MOEDAPGTO AS M ON CX.CDMOEDAPGT = M.CDMOEDAPGT " +
                    "LEFT JOIN SESCTO_CIELO AS CI ON CX.SQDEPRET = CI.SQDEPRET AND CX.SQCAIXA = CI.CAIXA AND CI.CANCELADO != 1 " +
                    "WHERE CX.CDPESSOA = @CDPESSOA AND CX.SQCAIXA = @SQCAIXA ORDER BY CX.SQCAIXA ASC, CX.SQDEPRET ASC";
                var lancamentos = await connection.QueryAsync<RecargaPaga>(
                    sql,
                    caixa
                );
                return lancamentos.AsList();
            }
        }
    }
}