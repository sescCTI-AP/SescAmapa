using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Pix;

namespace PagamentoApi.Repositories
{
    public class CobrancaRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger _logger;
        public CobrancaRepository(IConfiguration configuration, ILogger<CobrancaRepository> logger)
        {
            this.configuration = configuration;
            _logger = logger;
        }
        public List<COBRANCA> getCobrancas(CLIENTELA cliente, int ano)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobrancas = connection.Query<COBRANCA>(@"SELECT * FROM COBRANCA WHERE CDUOP = @cduop AND SQMATRIC = @sqmatric AND STRECEBIDO != 2 AND YEAR(DTVENCTO) = @ano",
                    new
                    {
                        cduop = cliente.CDUOP,
                        sqmatric = cliente.SQMATRIC,
                        ano = ano
                    });
                return (List<COBRANCA>)cobrancas;
            }
        }
        public async Task<List<CobrancaValor>> obterCobrancasEmAberto(CLIENTELA cliente, int ano)
        {
            var cobrancasValor = new List<CobrancaValor>();
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobrancas = connection.Query<COBRANCA>(@"SELECT * FROM COBRANCA WHERE CDUOP = @cduop AND SQMATRIC = @sqmatric AND STRECEBIDO = 0 AND YEAR(DTVENCTO) = @ano",
                    new
                    {
                        cduop = cliente.CDUOP,
                        sqmatric = cliente.SQMATRIC,
                        ano = ano
                    });
                foreach (var cobranca in cobrancas)
                {
                    var valorCobranca = await GetValorCobrancaComJurosMulta(cobranca);
                    cobrancasValor.Add(valorCobranca);
                }
                return cobrancasValor;
            }
        }
        public async Task<bool> VerificaInadimplencia( int cduop, int sqmatric)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    connection.Open();
                    var cobrancasVencidasCliente = await connection.QueryAsync<COBRANCA>(@"SELECT * FROM COBRANCA WHERE CDUOP = @cdUop and SQMATRIC = @sqMatric and 
                        DTVENCTO < current date and STRECEBIDO = 0 and IDCLASSE = 'OCRID'",
                        new
                        {
                            cdUop = cduop,
                            sqMatric = sqmatric
                        });
                    if (cobrancasVencidasCliente.Count() > 0)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public async Task<bool> VerificarCobrancaEstaPaga(string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    connection.Open();
                    var response = await connection.QueryFirstAsync<bool>(@"SELECT STRECEBIDO FROM COBRANCA WHERE CDELEMENT = @cdElement and SQCOBRANCA = @SqCobranca ",
                        new
                        {
                            cdElement = cdelement,
                            SqCobranca = sqcobranca
                        });
                    return response;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<PixCriado> VerificarCobrancaPixEstaAtivo(string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    connection.Open();
                    var response = await connection.QueryFirstOrDefaultAsync<PixCriado>(@"SELECT TXID, QRCODE as TextoImagemQRcode FROM DB2DBA.SESCTO_PIX WHERE CDELEMENT = @cdElement and SQCOBRANCA = @SqCobranca AND STATUS = 'ATIVA' ",
                        new
                        {
                            cdElement = cdelement,
                            SqCobranca = sqcobranca
                        });
                    return response;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        public async Task<List<ClientelaCobranca>> obterInadimplentes(ClienteInadimplente cliente)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    connection.Open();
                    var clientesCobrancas = new List<ClientelaCobranca>();
                    if (cliente.cduop > 0)
                    {
                        var query = "";
                        if (!string.IsNullOrEmpty(cliente.cpf))
                        {
                            query = @"select c.*, u.NMUOP from CLIENTELA c inner join UOP u on c.CDUOP = u.CDUOP
                where (select count(*) from COBRANCA co  where year(co.dtvencto) >= 2013 and co.dtvencto < current date 
                and co.STRECEBIDO = 0 and co.IDCLASSE = 'OCRID' and co.CDUOP = c.CDUOP and co.SQMATRIC = c.SQMATRIC) > 0 [[consulta-por-cpf]] order by c.nmcliente";

                            query = query.Replace("[[consulta-por-cpf]]", !string.IsNullOrEmpty(cliente.cpf) ? $" and c.NUCPF = '{cliente.cpf}' " : "");
                            var clientesCobrancasVencida = await connection.QueryAsync<CobrancaCliente>(query);

                            foreach (var item in clientesCobrancasVencida)
                            {
                                var cobrancasVencidasCliente = await connection.QueryAsync<COBRANCA>(@"SELECT * FROM COBRANCA WHERE CDUOP = @cdUop and SQMATRIC = @sqMatric and 
                        DTVENCTO < current date and STRECEBIDO = 0 and IDCLASSE = 'OCRID'",
                                    new
                                    {
                                        cdUop = item.CDUOP,
                                        sqMatric = item.SQMATRIC
                                    });
                                if (cobrancasVencidasCliente.Any())
                                {
                                    clientesCobrancas.Add(new ClientelaCobranca
                                    {
                                        NmCliente = item.NMCLIENTE,
                                        MatFormat = $"{item.CDUOP.ToString().PadLeft(4)}-{item.SQMATRIC.ToString().PadLeft(6)}-{item.NUDV}",
                                        Unidade = item.NMUOP,
                                        Cobrancas = cobrancasVencidasCliente.ToList()
                                    });
                                }
                            }
                        }
                        else
                        {
                            query = @"select c.*, u.NMUOP from CLIENTELA c inner join UOP u on c.CDUOP = u.CDUOP
                                where (select count(*) from COBRANCA co  where co.dtvencto < current date and co.STRECEBIDO = 0 
                                and co.IDCLASSE = 'OCRID' and co.CDUOP = @cdUop and co.CDUOP = c.CDUOP and co.SQMATRIC = c.SQMATRIC) > 0 
                                [[consulta-por-nome]] [[consulta-por-cpf]] order by c.nmcliente";

                            query = query.Replace("[[consulta-por-nome]]", !string.IsNullOrEmpty(cliente.nome) ? $" and c.NMCLIENTE like '%{cliente.nome.ToUpper()}%' " : "");
                            query = query.Replace("[[consulta-por-cpf]]", !string.IsNullOrEmpty(cliente.cpf) ? $" and c.NUCPF = '{cliente.cpf}' " : "");
                            var clientesCobrancasVencida = await connection.QueryAsync<CobrancaCliente>(query, new { CDUOP = cliente.cduop });

                            foreach (var item in clientesCobrancasVencida)
                            {

                                var sql = "SELECT * FROM COBRANCA WHERE CDUOP = @CDUOPCLI AND SQMATRIC = @SQMATRICCLI AND DTVENCTO >= @dataInicio AND DTVENCTO < current date AND STRECEBIDO = 0 AND IDCLASSE = 'OCRID'";
                                var cobrancasVencidasCliente = await connection.QueryAsync<COBRANCA>(sql,
                                    new
                                    {
                                        CDUOPCLI = item.CDUOP,
                                        SQMATRICCLI = item.SQMATRIC,
                                        dataInicio = cliente.dataInicial,
                                        dataFim = cliente.dataInicial
                                    });
                                if (cobrancasVencidasCliente.Any())
                                {
                                    clientesCobrancas.Add(new ClientelaCobranca
                                    {
                                        NmCliente = item.NMCLIENTE,
                                        MatFormat = $"{item.CDUOP.ToString().PadLeft(4)}-{item.SQMATRIC.ToString().PadLeft(6)}-{item.NUDV}",
                                        Unidade = item.NMUOP,
                                        Cobrancas = cobrancasVencidasCliente.ToList()
                                    });
                                }
                            }
                        }
                    }

                    return clientesCobrancas;
                }
                catch (Exception e)
                {
                    throw;
                }

            }
        }

        public async Task<List<COBRANCA>> obterCobrancasEmAberto(CLIENTELA cliente)
        {
            var cobrancasValor = new List<CobrancaValor>();
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobrancas = await connection.QueryAsync<COBRANCA>(@"SELECT * FROM COBRANCA WHERE CDUOP = @cduop AND SQMATRIC = @sqmatric AND STRECEBIDO = 0 ",
                    new
                    {
                        cduop = cliente.CDUOP,
                        sqmatric = cliente.SQMATRIC
                    });

                return cobrancas.ToList();
            }
        }

        public async Task<List<CobrancaValor>> obterProximasCobrancasEmAberto(string cpf)
        {
            var cobrancasValor = new List<CobrancaValor>();
            var dataLimite = DateTime.Now.AddDays(15);
            var cobrancas = new List<COBRANCA>();
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cliente = await connection.QueryFirstAsync<CLIENTELA>(@"SELECT CDUOP, SQMATRIC FROM CLIENTELA WHERE NUCPF = @cpf AND STMATRIC = 0 ", new { cpf });

                var allCobrancas = await connection.QueryAsync<COBRANCA>(@"SELECT * FROM COBRANCA WHERE CDUOP = @cduop AND SQMATRIC = @sqmatric AND STRECEBIDO = 0",
                     new
                     {
                         cduop = cliente.CDUOP,
                         sqmatric = cliente.SQMATRIC,
                         dataLimite
                     });

                if (allCobrancas.Any())
                {
                    var cobrancaProxima = allCobrancas.Where(c => c.DTVENCTO >= DateTime.Now.Date).OrderBy(c => c.DTVENCTO).FirstOrDefault();
                    cobrancas = allCobrancas.Where(c => c.DTVENCTO < DateTime.Now.Date).ToList();
                    if(cobrancaProxima != null)
                        cobrancas.Add(cobrancaProxima);
                }


                foreach (var cobranca in cobrancas)
                {
                    var valorCobranca = await GetValorCobrancaComJurosMulta(cobranca);
                    cobrancasValor.Add(valorCobranca);
                }

                return cobrancasValor.ToList();
            }
        }

        public async Task<CobrancaValor> ObterCobranca(string idclasse, string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobranca = await connection.QueryFirstOrDefaultAsync<COBRANCA>(@"SELECT * FROM COBRANCA WHERE IDCLASSE = @idclasse AND CDELEMENT = @cdelement AND SQCOBRANCA = @sqcobranca",
                    new
                    {
                        idclasse,
                        cdelement,
                        sqcobranca
                    });
                var valorCobranca = await GetValorCobrancaComJurosMulta(cobranca);

                return valorCobranca;
            }
        }
        public async Task<bool> VerificaCobranca(string idclasse, string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobranca = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT SQCOBRANCA FROM COBRANCA WHERE IDCLASSE = @idclasse AND CDELEMENT = @cdelement AND SQCOBRANCA = @sqcobranca",
                    new
                    {
                        idclasse,
                        cdelement,
                        sqcobranca
                    });
                    if (cobranca > 0)
                    return true;
                return false;
            }
        }

        public async Task<bool> IsCobrancaJaPaga(string idclasse, string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobranca = await connection.QueryFirstOrDefaultAsync<bool>(@"SELECT COUNT(STRECEBIDO) FROM COBRANCA WHERE IDCLASSE = @idclasse AND CDELEMENT = @cdelement AND SQCOBRANCA = @sqcobranca AND STRECEBIDO != 0 ",
                    new
                    {
                        idclasse,
                        cdelement,
                        sqcobranca
                    });
                if (cobranca)
                    return true;

                return false;
            }
        }

        public async Task<dynamic> ObterCobrancasPagasPorCaixa(CACAIXA caixa)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sql = "SELECT TRIM(CF.MATFORMAT) AS Habilitacao, TRIM(CLI.NMCLIENTE) AS Cliente, P.VLRECEBIDO AS Valor, P.VLACRESCIM AS Acrescimo, P.VLDESCONTO As Desconto, P.VLJUROS AS Juros, " +
                    " M.DSMOEDAPGT AS Moeda, TRIM(C.DSCOBRANCA) AS Atividade, C.DTVENCTO AS Vencimento, CI.TID " +
                    " FROM PAGAMENTOS AS P " +
                    " INNER JOIN COBRANCA AS C ON P.CDELEMENT = C.CDELEMENT AND P.SQCOBRANCA = C.SQCOBRANCA " +
                    " INNER JOIN CLIENTELA AS CLI ON C.CDUOP = CLI.CDUOP AND C.SQMATRIC = CLI.SQMATRIC " +
                    " INNER JOIN CLIFORMAT AS CF ON C.CDUOP = CF.CDUOP AND C.SQMATRIC = CF.SQMATRIC " +
                    " INNER JOIN MOEDAPGTO AS M ON P.CDMOEDAPGT = M.CDMOEDAPGT " +
                    " LEFT JOIN SESCTO_CIELO AS CI ON P.CDELEMENT = CI.CDELEMENT AND  P.SQCOBRANCA = CI.SQCOBRANC " +
                    " WHERE P.SQCAIXA = @SQCAIXA AND P.CDPESSOA = @CDPESSOA ";
                var pagamentos = await connection.QueryAsync<CobrancaPaga>(sql,

                    caixa);

                return pagamentos;
            }
        }
        public async Task<COBRANCA> ObterCobrancaCompleta(string idclasse, string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobranca = await connection.QueryFirstAsync<COBRANCA>(@"SELECT * FROM COBRANCA WHERE IDCLASSE = @idclasse AND CDELEMENT = @cdelement AND SQCOBRANCA = @sqcobranca",
                    new
                    {
                        idclasse,
                        cdelement,
                        sqcobranca
                    });
                //var valorCobranca = GetValorCobrancaComJurosMulta(cobranca);

                return cobranca;
            }
        }
        public async Task<CobrancaValor> GetValorCobrancaComJurosMulta(COBRANCA cobranca)
        {
            var descacres = await ObterDescontoAcrescimo(cobranca);

            var cobrancaValor = new CobrancaValor();
            cobrancaValor.IDCLASSE = cobranca.IDCLASSE;
            cobrancaValor.CDELEMENT = cobranca.CDELEMENT.Trim();
            cobrancaValor.SQCOBRANCA = cobranca.SQCOBRANCA;
            cobrancaValor.ValorOriginal = cobranca.VLCOBRADO;
            cobrancaValor.Atividade = cobranca.DSCOBRANCA.Trim();
            cobrancaValor.Vencimento = cobranca.DTVENCTO;
            cobrancaValor.JurosMora = 0.00M;
            cobrancaValor.Multa = 0.00M;

            var qtdDiasVencidos =
                decimal.Parse(
                    (DateTime.Now.Date - cobranca.DTVENCTO.Date).TotalDays.ToString(CultureInfo.InvariantCulture));
            cobrancaValor.ValorRecebido = cobranca.VLCOBRADO;            

            if (qtdDiasVencidos < 0) //Obtem desconto da tabela do site
            {
                var cdprograma = Convert.ToInt32(cobranca.CDELEMENT.Substring(0, 8));
                var cdconfig = Convert.ToInt32(cobranca.CDELEMENT.Substring(8, 8));
                var sqocorrenc = Convert.ToInt32(cobranca.CDELEMENT.Substring(16, 8));
                using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
                {
                    var sql = "SELECT DescontoPontualidade FROM AtividadeOnLine WHERE CDPROGRAMA = @CDPROGRAMA " +
                        " AND SQOCORRENC = @SQOCORRENC AND CDCONFIG = @CDCONFIG";
                    var desconto = await connection.QueryFirstOrDefaultAsync<decimal>(sql, new
                    {
                        cdprograma,
                        cdconfig,
                        sqocorrenc
                    });
                    if (desconto > 0)
                    {
                        cobrancaValor.DescontoConcedido = Math.Round((cobranca.VLCOBRADO * desconto), 2, MidpointRounding.AwayFromZero);
                        cobrancaValor.ValorRecebido -= cobrancaValor.DescontoConcedido;
                    }
                }
            }

            if (descacres != null)
            {
                if (descacres.RFDIASLMT < 0 &&
                    (Math.Abs(qtdDiasVencidos) > descacres.DDLIMITE ||
                        ((Math.Abs(qtdDiasVencidos) >= 0 && cobrancaValor.ValorOriginal > 1000))
                    )
                ) //Dias antes do vencimento 
                {
                    if (descacres.TPLANCAMEN == 0)
                    { //Desconto
                        if (descacres.TPVALOR == 0)
                        { //Especie
                            cobrancaValor.DescontoConcedido = descacres.VLDESCACRE;
                            cobrancaValor.ValorRecebido -= cobrancaValor.DescontoConcedido;
                        }
                        else if (descacres.TPVALOR == 1)
                        { //Percentual
                            cobrancaValor.DescontoConcedido = cobranca.VLCOBRADO * descacres.VLDESCACRE / 100;
                            cobrancaValor.ValorRecebido -= cobrancaValor.DescontoConcedido;
                        }
                    }
                    // else if (descacres.TPLANCAMEN == 1)
                    // { //Acrescimo
                    //     if (descacres.TPVALOR == 0)
                    //     { //Especie
                    //         cobrancaValor.Acrescimo = descacres.VLDESCACRE;
                    //         cobrancaValor.ValorRecebido += cobrancaValor.DescontoConcedido;
                    //     }
                    //     else if (descacres.TPVALOR == 1)
                    //     { //Percentual
                    //         cobrancaValor.Acrescimo = cobranca.VLCOBRADO * (100 - descacres.VLDESCACRE) / 100;
                    //         cobrancaValor.ValorRecebido += cobrancaValor.DescontoConcedido;
                    //     }
                    // }
                }
                else if (qtdDiasVencidos >= descacres.DDLIMITE)
                { //Depois do vencimento
                    // if (descacres.TPLANCAMEN == 0)
                    // { //Desconto
                    //     if (descacres.TPVALOR == 0)
                    //     { //Especie
                    //         cobrancaValor.DescontoConcedido = descacres.VLDESCACRE;
                    //         cobrancaValor.ValorRecebido -= cobrancaValor.DescontoConcedido;
                    //     }
                    //     else if (descacres.TPVALOR == 1)
                    //     { //Percentual
                    //         cobrancaValor.DescontoConcedido = cobranca.VLCOBRADO * descacres.VLDESCACRE / 100;
                    //         cobrancaValor.ValorRecebido -= cobrancaValor.DescontoConcedido;
                    //     }
                    // } else 
                    if (descacres.TPLANCAMEN == 1)
                    { //Acrescimo
                        if (descacres.TPVALOR == 0)
                        { //Especie
                            cobrancaValor.Acrescimo = descacres.VLDESCACRE;
                            cobrancaValor.ValorRecebido += cobrancaValor.DescontoConcedido;
                        }
                        else if (descacres.TPVALOR == 1)
                        { //Percentual
                            cobrancaValor.Acrescimo = cobranca.VLCOBRADO * (100 - descacres.VLDESCACRE) / 100;
                            cobrancaValor.ValorRecebido += cobrancaValor.DescontoConcedido;
                        }
                    }
                }
            }
            
            if (qtdDiasVencidos > 0)
            {
                // Caso o vencimento seja no sábado ou domingo, o pagamento pode ser feito ate segunda, sem juros ou multa               
                if(qtdDiasVencidos <= 2 && (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Monday))
                    return cobrancaValor;

                // Regra para ser executada somente dia 02.05.2024 - Pos Feriado de dia do Trabalho
                
                if (qtdDiasVencidos == 1 && DateTime.Now.Date == new DateTime(2024, 05, 02))
                    return cobrancaValor;
                

                // Regra para ser executada somente dia 02.01.2024 - Pos Feriado de Ano Novo
                /*
                if (qtdDiasVencidos == 1 && DateTime.Now.Date == new DateTime(2024, 01, 02))
                    return cobrancaValor;
                */

                if (cobranca.PCJUROS != null)
                {
                    var porcentagemJurosMora = cobranca.PCJUROS / 100;
                    var valorComJurosMora = ((cobranca.VLCOBRADO * porcentagemJurosMora) * qtdDiasVencidos);
                    cobrancaValor.JurosMora = Math.Round(valorComJurosMora.Value, 2, MidpointRounding.AwayFromZero);
                    cobrancaValor.ValorRecebido += cobrancaValor.JurosMora;
                }

                if (cobranca.PCMULTA != null)
                {
                    var porcentagemMulta = cobranca.PCMULTA / 100;
                    var valorNominalComMulta = (cobranca.VLCOBRADO * porcentagemMulta);
                    cobrancaValor.Multa = Math.Round(valorNominalComMulta.Value, 2, MidpointRounding.AwayFromZero);
                    cobrancaValor.ValorRecebido += cobrancaValor.Multa;
                }
            }

            return cobrancaValor;
        }

        /// <summary>
        /// Gera registro de pagamento para cobrança
        /// </summary>
        /// <param name="cobranca"></param>
        /// <param name="caixa"></param>
        /// <param name="cobrancaValor"></param>
        /// <returns></returns>
        public async Task<dynamic> PagamentoCobranca(CACAIXA caixa, CobrancaValor cobranca, DB2Connection connection, DbTransaction transaction, int moeda = 3)
        {
            var cobrancaEstaPaga = await VerificarCobrancaEstaPaga(cobranca.CDELEMENT, cobranca.SQCOBRANCA);
            if(cobrancaEstaPaga)
                return false;

            var sqPagamento = await this.RetornaSequencialPagamento(caixa, moeda);
            var pagamento = new PAGAMENTOS
            {
                DTRECEBIDO = DateTime.Now.Date,
                SQCAIXA = caixa.SQCAIXA,
                HRRECEBIDO = DateTime.Now.TimeOfDay,
                CDMOEDAPGT = moeda,
                SQPAGAMENT = sqPagamento + 1,
                IDCLASSE = cobranca.IDCLASSE,
                CDELEMENT = cobranca.CDELEMENT,
                SQCOBRANCA = cobranca.SQCOBRANCA,

                VLRECEBIDO = cobranca.ValorRecebido,
                VLJUROS = cobranca.JurosMora + cobranca.Multa,
                VLACRESCIM = cobranca.Acrescimo,
                VLDESCONTO = cobranca.DescontoConcedido,

                NUIMPVIA2 = 0,
                CDUOPPGTO = caixa.CDUOP,
                SQVENDA = 0,
                SMFIELDATU = 0,
                STCANCELAD = 0,
                CDPESSOA = caixa.CDPESSOA
            };
            // using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            // {
            //await connection.OpenAsync();

            // try
            // {
            var sql = "INSERT INTO PAGAMENTOS (DTRECEBIDO, SQCAIXA, HRRECEBIDO, CDMOEDAPGT, SQPAGAMENT, IDCLASSE, CDELEMENT, SQCOBRANCA, VLRECEBIDO, VLJUROS, SMFIELDATU, " +
                "STCANCELAD, VLACRESCIM, VLDESCONTO, NUIMPVIA2, CDUOPPGTO, SQVENDA, CDPESSOA, IDCOBRANCA ) " +
                " VALUES (@DTRECEBIDO, @SQCAIXA, @HRRECEBIDO, @CDMOEDAPGT, @SQPAGAMENT, @IDCLASSE, @CDELEMENT, @SQCOBRANCA, @VLRECEBIDO, @VLJUROS, @SMFIELDATU, " +
                " @STCANCELAD, @VLACRESCIM, @VLDESCONTO, @NUIMPVIA2, @CDUOPPGTO, @SQVENDA, @CDPESSOA, @IDCOBRANCA ) ";

            await connection.ExecuteAsync(
                sql,
                pagamento,
                transaction
            );

            sql = "UPDATE COBRANCA SET STRECEBIDO = @STRECEBIDO, DTATU = current date, HRATU = current time, LGATU = @LGATU WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND SQCOBRANCA = @SQCOBRANCA";
            await connection.ExecuteAsync(
                sql,
                new
                {
                    STRECEBIDO = 1,
                    LGATU = Convert.ToString(caixa.CDPESSOA),
                    IDCLASSE = cobranca.IDCLASSE,
                    CDELEMENT = cobranca.CDELEMENT,
                    SQCOBRANCA = cobranca.SQCOBRANCA
                },
                transaction
            );
            //await transaction.CommitAsync();
            return "";
            //}
            // catch (Exception e)
            // {
            //     await transaction.RollbackAsync();
            //     Console.WriteLine("Erro ao pagar cobrança: " + e.Message);
            //     return "Erro ao inserir pagamento";
            // }
            //}
        }

        public async Task<int> RetornaSequencialPagamento(CACAIXA caCaixa, int cdMoedaPgto)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT SQPAGAMENT FROM PAGAMENTOS WHERE CDPESSOA = @CDPESSOA AND SQCAIXA = @SQCAIXA AND CDMOEDAPGT = @CDMOEDAPGTO ORDER BY SQPAGAMENT DESC";
                var sqPagamento = await connection.QueryFirstOrDefaultAsync<int?>(sql,
                    new
                    {
                        CDPESSOA = caCaixa.CDPESSOA,
                        SQCAIXA = caCaixa.SQCAIXA,
                        CDMOEDAPGTO = cdMoedaPgto
                    });
                return sqPagamento != null ? sqPagamento++.Value : 1;
            }
        }

        public async Task<DESCACRES> ObterDescontoAcrescimo(COBRANCA cobranca)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var cdprograma = Convert.ToInt32(cobranca.CDELEMENT.Substring(0, 8));
                var cdconfig = Convert.ToInt32(cobranca.CDELEMENT.Substring(8, 8));
                var sqocorrenc = Convert.ToInt32(cobranca.CDELEMENT.Substring(16, 8));

                var sql = "SELECT CDPROGRAMA, CDCONFIG, SQOCORRENC, CDPERFIL, CDFORMATO FROM INSCRICAO " +
                    "WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC AND CDUOP = @CDUOP AND SQMATRIC = @SQMATRIC";
                var inscricao = await connection.QuerySingleOrDefaultAsync<INSCRICAO>(sql,
                    new
                    {
                        cdprograma,
                        cdconfig,
                        sqocorrenc,
                        cobranca.CDUOP,
                        cobranca.SQMATRIC
                    });

                sql = "SELECT DDLIMITE, TPLANCAMEN, TPVALOR, VLDESCACRE, RFDIASLMT FROM DESCACRES " +
                    "WHERE CDPROGRAMA = @CDPROGRAMA AND CDCONFIG = @CDCONFIG AND SQOCORRENC = @SQOCORRENC AND CDPERFIL = @CDPERFIL AND CDFORMATO = @CDFORMATO";
                var descacres = await connection.QuerySingleOrDefaultAsync<DESCACRES>(sql,
                    inscricao);
                return descacres;
            }

        }

        /// <summary>
        /// Insere registro pagamento cielo
        /// </summary>
        /// <param name="pagamentoCielo"></param>
        /// <returns></returns>
        public async Task<dynamic> PagamentoCobrancaCielo(PagamentoCielo pagamentoCielo, CACAIXA caixa, CobrancaValor cobranca)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var sql = "INSERT INTO SESCTO_CIELO (IDCLASSE, CDELEMENT, SQCOBRANC, MERCHANTORDER, CARDNUMBER, BRAND, PROOFOFSALE, TID, AUTHORIZATIONCODE, " +
                            " PAYMENTID, TIPO, VALOR, PARCELAS, DTOPERACAO, CAIXA, CDPESSOA, CANCELADO) " +
                            " VALUES (@IDCLASSE, @CDELEMENT, @SQCOBRANC, @MERCHANTORDER, @CARDNUMBER, @BRAND, @PROOFOFSALE, @TID, @AUTHORIZATIONCODE, " +
                            " @PAYMENTID, @TIPO, @VALOR, @PARCELAS, @DTOPERACAO, @CAIXA, @CDPESSOA, @CANCELADO) ";

                        await connection.ExecuteAsync(
                            sql,
                            pagamentoCielo,
                            transaction
                        );
                        var cdmoeda = pagamentoCielo.PARCELAS > 1 ? 4 : 3;
                        var sqPagamento = await this.RetornaSequencialPagamento(caixa, cdmoeda);
                        var pagamento = new PAGAMENTOS
                        {
                            DTRECEBIDO = DateTime.Now.Date,
                            SQCAIXA = caixa.SQCAIXA,
                            HRRECEBIDO = DateTime.Now.TimeOfDay,
                            CDMOEDAPGT = cdmoeda,
                            SQPAGAMENT = sqPagamento + 1,
                            IDCLASSE = cobranca.IDCLASSE,
                            CDELEMENT = cobranca.CDELEMENT,
                            SQCOBRANCA = cobranca.SQCOBRANCA,

                            VLRECEBIDO = cobranca.ValorRecebido,
                            VLJUROS = cobranca.JurosMora + cobranca.Multa,
                            VLACRESCIM = cobranca.Acrescimo,
                            VLDESCONTO = cobranca.DescontoConcedido, //TODO: Pegar desconto concedido pelo site

                            NUIMPVIA2 = 0,
                            CDUOPPGTO = caixa.CDUOP,
                            SQVENDA = 0,
                            SMFIELDATU = 0,
                            STCANCELAD = 0,
                            CDPESSOA = caixa.CDPESSOA
                        };
                        sql = "INSERT INTO PAGAMENTOS (DTRECEBIDO, SQCAIXA, HRRECEBIDO, CDMOEDAPGT, SQPAGAMENT, IDCLASSE, CDELEMENT, SQCOBRANCA, VLRECEBIDO, VLJUROS, SMFIELDATU, " +
                            "STCANCELAD, VLACRESCIM, VLDESCONTO, NUIMPVIA2, CDUOPPGTO, SQVENDA, CDPESSOA, IDCOBRANCA ) " +
                            " VALUES (@DTRECEBIDO, @SQCAIXA, @HRRECEBIDO, @CDMOEDAPGT, @SQPAGAMENT, @IDCLASSE, @CDELEMENT, @SQCOBRANCA, @VLRECEBIDO, @VLJUROS, @SMFIELDATU, " +
                            " @STCANCELAD, @VLACRESCIM, @VLDESCONTO, @NUIMPVIA2, @CDUOPPGTO, @SQVENDA, @CDPESSOA, @IDCOBRANCA ) ";

                        await connection.ExecuteAsync(
                            sql,
                            pagamento,
                            transaction
                        );

                        sql = "UPDATE COBRANCA SET STRECEBIDO = @STRECEBIDO, DTATU = current date, HRATU = current time, LGATU = @LGATU WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND SQCOBRANCA = @SQCOBRANCA";
                        await connection.ExecuteAsync(
                            sql,
                            new
                            {
                                STRECEBIDO = 1,
                                LGATU = Convert.ToString(caixa.CDPESSOA),
                                IDCLASSE = cobranca.IDCLASSE,
                                CDELEMENT = cobranca.CDELEMENT,
                                SQCOBRANCA = cobranca.SQCOBRANCA
                            },
                            transaction
                        );
                        await transaction.CommitAsync();
                        return "";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine("Erro ao salvar cobrança cielo: " + e.Message);
                        _logger.LogError("Erro ao salvar cobrançca cielo");
                        _logger.LogError(e.Message);
                        return "Erro ao inserir cobrança cielo";
                    }
                }
            }
        }

        public async Task<string> CancelaCobrancas(List<string> turmas)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var turma in turmas)
                        {
                            var turmaArray = turma.Split('.');
                            var CDPROGRAMA = turmaArray[0];
                            var CDCONFIG = turmaArray[1];
                            var SQOCORRENC = turmaArray[2];

                            var cdElement = CDPROGRAMA.ToString().PadLeft(8, '0') + CDCONFIG.ToString().PadLeft(8, '0') + SQOCORRENC.ToString().PadLeft(8, '0');

                            var dataCancelamento = new DateTime(2020, 10, 15);
                            //string idclasse, string cdelement              
                            var sql = "UPDATE COBRANCA SET STRECEBIDO = 2, DTATU = current date, HRATU = current time, DSCANCELAM = @DSCANCELAM, CDCANCELA = 81, " +
                                " LGATU = @LGATU, LGCANCEL = @LGATU " +
                                "WHERE IDCLASSE = @IDCLASSE AND CDELEMENT = @CDELEMENT AND STRECEBIDO = 0 AND DTVENCTO = @dataCancelamento";
                            await connection.ExecuteAsync(
                                sql,
                                new
                                {
                                    DSCANCELAM = "Cancelamento automático Covid-19",
                                    IDCLASSE = "OCRID",
                                    CDELEMENT = cdElement,
                                    LGATU = "11821",
                                    dataCancelamento
                                },
                                transaction
                            );
                        }
                        await transaction.CommitAsync();
                        return "";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine("Erro ao cancelar cobranca: " + e.Message);
                        return "Erro ao cancelar cobrança";
                    }
                }
            }
        }
    }
}