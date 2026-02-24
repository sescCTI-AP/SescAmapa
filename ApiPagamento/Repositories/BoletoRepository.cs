using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Clients;
using PagamentoApi.Models;
using PagamentoApi.Models.BB;
using PagamentoApi.Models.Boleto;
using PagamentoApi.Settings;

namespace PagamentoApi.Repositories
{
    public class BoletoRepository
    {
        public readonly IConfiguration _configuration;
        private readonly BBApiSettings _apiSettings;
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly CaixaRepository _caixaRepository;
        private int _producao;

        //private readonly BBClient _bbClient;

        public BoletoRepository(IConfiguration configuration, BBApiSettings apiSettings, CobrancaRepository cobrancaRepository, CaixaRepository caixaRepository)
        {
            _configuration = configuration;
            _apiSettings = apiSettings;
            if (_apiSettings.Sandbox)
            {
                _producao = 0;
            }
            else
            {
                _producao = 1;
            }
            _cobrancaRepository = cobrancaRepository;
            _caixaRepository = caixaRepository;
            //_bbClient = bbClient;
        }
        /// <summary>
        /// Obtem o nosso número do banco de dados
        /// </summary>
        /// <returns></returns>
        public async Task<SescTO_NossoNumero> ObterNossoNumero(string convenio)
        {

            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var caixa = await connection.QueryAsync<SescTO_NossoNumero>(
                    @"SELECT * FROM DB2DBA.SESCTO_NOSSONUMERO WHERE PRODUCAO = @PRODUCAO ORDER BY NUMERO DESC",
                    new { PRODUCAO = _producao });
                return caixa.FirstOrDefault();
            }
        }
        /// <summary>
        /// Atualiza nosso numero no banco
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AtualizarNossoNumero(SescTO_NossoNumero nossoNumero)
        {

            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sql =
                    @"UPDATE DB2DBA.SESCTO_NOSSONUMERO SET NUMERO = @NUMERO, DATA_ULTIMA_ATUALIZACAO = CURRENT TIMESTAMP WHERE ID = @ID";
                var affectedRows = await connection.ExecuteAsync(sql, nossoNumero);
                if (affectedRows <= 0)
                {
                    return false;
                }
                else
                    return true;
            }
        }


        /// <summary>
        /// Atualiza nosso numero (incrementa 1) no banco e retorna o novo numero
        /// </summary>
        /// <returns></returns>
        public async Task<SescTO_NossoNumero> AtualizarObterNossoNumero()
        {
            try
            {
                using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var nossoNumero = await connection.QueryAsync<SescTO_NossoNumero>(
                            @"SELECT * FROM DB2DBA.SESCTO_NOSSONUMERO WITH (UPDLOCK) WHERE PRODUCAO = @PRODUCAO ORDER BY NUMERO DESC", new { PRODUCAO = _producao }, transaction);

                          //                        var nossoNumero = await connection.QueryAsync<SescTO_NossoNumero>(
                          //                          @"SELECT * FROM DB2DBA.SESCTO_NOSSONUMERO WITH (UPDLOCK) WHERE PRODUCAO = @PRODUCAO ORDER BY NUMERO DESC", new { PRODUCAO = _producao }, transaction);

                          /*

                          var sqlUpdate = @" UPDATE DB2DBA.SESCTO_NOSSONUMERO SET NUMERO = NUMERO + 1, DATA_ULTIMA_ATUALIZACAO = CURRENT TIMESTAMP WHERE PRODUCAO = @PRODUCAO ";

                          var affectedRows = await connection.ExecuteAsync(sqlUpdate, new { PRODUCAO = _producao }, transaction);

                          if (affectedRows <= 0)
                          {
                              transaction.Rollback();
                              return null;
                          }
                          */



                       //   transaction.Commit();

                        return nossoNumero.FirstOrDefault();

                    }
                }

            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                return null;
            }

        }

        /// <summary>
        /// Salva boleto gerado no Banco
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SalvarBoleto(SescTO_Boletos boleto)
        {
            boleto.PRODUCAO = _producao;

            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sql =
                    @"INSERT INTO DB2DBA.SESCTO_BOLETOS (SQMATRIC_CLIENTELA, CDUOP_CLIENTELA, CPF_RESPONSAVEL, NOSSO_NUMERO, " +
                    "BASE_NOSSO_NUMERO, VALOR_BOLETO, VALOR_JUROS_MULTA, VALOR_DESCONTO, DATA_VENCIMENTO, CONVENIO, " +
                    "IDCLASSE, CDELEMENT, SQCOBRANCA, STATUS, DATAGERACAO, LINHA_DIGITAVEL, CODIGO_BARRAS, QRCODE_URL, " +
                    "QRCODE_TXID, QRCODE_EMV, DATAATUALIZACAO, PRODUCAO) VALUES " +
                    "(@SQMATRIC_CLIENTELA, @CDUOP_CLIENTELA, @CPF_RESPONSAVEL, @NOSSO_NUMERO, " +
                    "@BASE_NOSSO_NUMERO, @VALOR_BOLETO, @VALOR_JUROS_MULTA, @VALOR_DESCONTO, @DATA_VENCIMENTO, @CONVENIO, " +
                    "@IDCLASSE, @CDELEMENT, @SQCOBRANCA, @STATUS, CURRENT TIMESTAMP, @LINHA_DIGITAVEL, @CODIGO_BARRAS, @QRCODE_URL, " +
                    "@QRCODE_TXID, @QRCODE_EMV, CURRENT TIMESTAMP, @PRODUCAO);";
                var affectedRows = await connection.ExecuteAsync(sql, boleto);
                if (affectedRows <= 0)
                {
                    return false;
                }
                else
                    return true;
            }
        }
        /// <summary>
        /// Marca os boletos como pagos
        /// </summary>
        /// <param name="boletosLiquidados"></param>
        /// <returns></returns>
        public async Task<dynamic> LiquidarBoletos(BBClient bbClient)
        {
            try
            {
                using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                {
                    connection.Open();
                    //var NossosNumeros = boletosLiquidados.Select(x => x.numeroBoletoBB);
                    var boletos = await connection.QueryAsync<SescTO_Boletos>(
                        @"SELECT * FROM DB2DBA.SESCTO_BOLETOS WHERE STATUS NOT IN (6,7) AND PRODUCAO = @PRODUCAO",
                        new { PRODUCAO = _producao }
                    );
                    var caixa = await _caixaRepository.ObterCaixaAberto();
                    if (!(caixa is CACAIXA))
                    {
                        caixa = await _caixaRepository.AbrirCaixa();
                        if (!(caixa is CACAIXA))
                            return "Não existe um caixa aberto";
                    }
                    foreach (var boleto in boletos)
                    {
                        using (var transaction = await connection.BeginTransactionAsync())
                        {
                            try
                            {
                                var boletoLiquidado = await bbClient.ObterDetalhesBoletos(boleto.NOSSO_NUMERO);
                                // var boletoLiquidado = await bbClient.ObterDetalhesBoletos("00028133869000024837");
                                // Console.WriteLine(boleto.NOSSO_NUMERO);

                                if (boletoLiquidado is DetalhamentoBoleto)
                                {
                                    //boletosLiquidados.First(b => b.numeroBoletoBB == boleto.REF_TRAN);
                                    //Se o boleto está pago marca a cobranca como paga, mas somente se o dia atual for igual a DataCreditoLiquidacao retornada pela API do Banco

                                    var IsdiaDeLiquidaBoleto = DateTime.Now.ToString("dd.MM.yyyy").CompareTo(boletoLiquidado.DataCreditoLiquidacao);

                                    if ( boletoLiquidado.CodigoEstadoTituloCobranca == 6 && IsdiaDeLiquidaBoleto == 0 )
                                    {
                                        var hasCobranca = await _cobrancaRepository.VerificaCobranca(boleto.IDCLASSE, boleto.CDELEMENT, boleto.SQCOBRANCA);
                                        if (hasCobranca)
                                        {
                                            var cobrancaValor = new CobrancaValor
                                            {
                                                CDELEMENT = boleto.CDELEMENT,
                                                IDCLASSE = boleto.IDCLASSE,
                                                SQCOBRANCA = boleto.SQCOBRANCA,
                                                ValorOriginal = boleto.VALOR_BOLETO,
                                                ValorRecebido = Convert.ToDecimal(boletoLiquidado.ValorPagoSacado),
                                                JurosMora = Convert.ToDecimal(boletoLiquidado.ValorJuroMoraRecebido),
                                                Multa = Convert.ToDecimal(boletoLiquidado.ValorMultaRecebido),
                                                DescontoConcedido = Convert.ToDecimal(boletoLiquidado.ValorDescontoUtilizado)
                                            };

                                            await _cobrancaRepository.PagamentoCobranca(caixa, cobrancaValor, connection, transaction, 1221);

                                            var sql = @"UPDATE DB2DBA.SESCTO_BOLETOS SET STATUS = @Status, DATAATUALIZACAO = CURRENT TIMESTAMP WHERE ID = @Id";
                                            var affectedRows = await connection.ExecuteAsync(sql,
                                                new
                                                {
                                                    Status = boletoLiquidado.CodigoEstadoTituloCobranca,
                                                    Id = boleto.ID
                                                },
                                                transaction
                                            );
                                        }
                                        
                                    }
                                    else if (boletoLiquidado.CodigoEstadoTituloCobranca == 1)
                                    {
                                        ///TODO: Baixar boletos cujas cobranças não existem
                                        var hasCobranca = await _cobrancaRepository.VerificaCobranca(boleto.IDCLASSE, boleto.CDELEMENT, boleto.SQCOBRANCA);
                                        if (!hasCobranca)
                                        {
                                            var retornoBaixa = await bbClient.BaixarBoleto(boleto.NOSSO_NUMERO);
                                            Console.WriteLine(retornoBaixa);
                                        }
                                    }
                                    else if (boletoLiquidado.CodigoEstadoTituloCobranca == 7)
                                    {
                                        var sql =
                                                @"UPDATE DB2DBA.SESCTO_BOLETOS SET STATUS = 7, DATAATUALIZACAO = CURRENT TIMESTAMP WHERE ID = @Id";
                                        var affectedRows = await connection.ExecuteAsync(sql,
                                            new { Id = boleto.ID },
                                            transaction);
                                    }
                                }
                                // else
                                // {
                                //     var sql =
                                //             @"UPDATE DB2DBA.SESCTO_BOLETOS SET STATUS = @Status, DATAATUALIZACAO = CURRENT TIMESTAMP WHERE ID = @Id";
                                //     var affectedRows = await connection.ExecuteAsync(sql,
                                //         new
                                //         {
                                //             Status = 99,
                                //             Id = boleto.ID
                                //         },
                                //         transaction
                                //     );
                                // }
                                await transaction.CommitAsync();
                            }
                            catch (Exception e)
                            {
                                await transaction.RollbackAsync();
                                return $"Erro ao atualizar boletos: {e.Message}";
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Marca os boletos como pagos
        /// </summary>
        /// <param name="boletosLiquidados"></param>
        /// <returns></returns>
        public async Task<dynamic> BaixarBoletos(List<Boleto> boletosLiquidados, BBClient bbClient)
        {
            try
            {
                using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                {
                    connection.Open();
                    var NossosNumeros = boletosLiquidados.Select(x => x.numeroBoletoBB);
                    var boletos = await connection.QueryAsync<SescTO_Boletos>(
                        @"SELECT * FROM DB2DBA.SESCTO_BOLETOS WHERE NOSSO_NUMERO IN @NossosNumeros",
                        new { NossosNumeros }, null);
                    if (boletos.Count() > 0)
                    {
                        var sql =
                            @"UPDATE DB2DBA.SESCTO_BOLETOS SET STATUS = 7, DATAATUALIZACAO = CURRENT TIMESTAMP WHERE  ID IN @Ids";
                        var affectedRows = await connection.ExecuteAsync(sql,
                            new { Ids = boletos.Select(b => b.ID) });
                        if (affectedRows <= 0)
                        {
                            return false;
                        }
                        else
                            return true;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Verifica se o boleto foi gerado e esta com data de vencimento válida
        /// </summary>
        /// <param name="verificaBoleto"></param>
        /// <returns></returns>
        public async Task<SescTO_Boletos> VerificaBoleto(string cdelement, int sqcobranca)
        {
            try
            {
                using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                {
                    connection.Open();
                    var boletos = await connection.QueryAsync<SescTO_Boletos>(
                        @"SELECT * FROM DB2DBA.SESCTO_BOLETOS WHERE CDELEMENT = @cdelement and SQCOBRANCA = @sqcobranca AND STATUS = 1",
                        new { cdelement = cdelement, sqcobranca = sqcobranca });
                    if (boletos.Count() > 0)
                    {
                        if (boletos.Any(a => a.DATA_VENCIMENTO.Date >= DateTime.Now.Date))
                        {
                            return boletos.FirstOrDefault(a => a.DATA_VENCIMENTO.Date >= DateTime.Now.Date);
                        }
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
        }

        // VALIDAR COM O MATHEUS 02.04.2024
        public async Task<SescTO_Boletos> BaixaBoleto(string cdelement, int sqcobranca)
        {
            try
            {
                using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                {
                    connection.Open();
                    var boletos = await connection.QueryAsync<SescTO_Boletos>(
                        @"UPDATE DB2DBA.SESCTO_BOLETOS SET STATUS = 7 WHERE CDELEMENT = @cdelement and SQCOBRANCA = @sqcobranca ",
                        new { cdelement = cdelement, sqcobranca = sqcobranca });
                    /*
                    if (boletos.Count() > 0)
                    {
                        if (boletos.Any(a => a.DATA_VENCIMENTO.Date >= DateTime.Now.Date))
                        {
                            return boletos.FirstOrDefault(a => a.DATA_VENCIMENTO.Date >= DateTime.Now.Date);
                        }
                    }*/
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
        }
    }
}