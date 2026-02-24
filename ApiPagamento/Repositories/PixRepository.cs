using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Clients;
using PagamentoApi.Models;
using PagamentoApi.Models.Pix;
using PagamentoApi.Services;
using PagamentoApi.Settings;

namespace PagamentoApi.Repositories
{
    public class PixRepository
    {
        public readonly IConfiguration _configuration;
        private readonly BBApiSettings _apiSettings;
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly CaixaRepository _caixaRepository;
        private readonly CartaoRepository _cartaoRepository;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ApiBancoBrasilService _apiBancoBrasilService;

        private readonly SiteRepository _siteRepository;
        private int _producao;

        public PixRepository(IConfiguration configuration, BBApiSettings apiSettings, CobrancaRepository cobrancaRepository, CaixaRepository caixaRepository, CartaoRepository cartaoRepository, SiteRepository siteRepository, SemaphoreSlim semaphoreSlim, ApiBancoBrasilService apiBancoBrasilService)
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
            _cartaoRepository = cartaoRepository;
            _siteRepository = siteRepository;
            _semaphoreSlim = semaphoreSlim;
            _apiBancoBrasilService = apiBancoBrasilService;
            //_bbClient = bbClient;
        }
        public async Task<dynamic> ListarPix()
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = "SELECT P.* FROM DB2DBA.SESCTO_PIX AS P" +
                        " LEFT JOIN DB2DBA.SESCTO_PIX_TRANSACOES AS T ON T.TXID = P.TXID ORDER BY P.CRIACAO DESC, T.HORARIO";
                    var moviment = await connection.QueryAsync<SescTO_Pix>(sql, new { });
                    return moviment.AsList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        public async Task<List<SescTO_Pix>> ListarPixAtivos(string txId = null)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();               
                    string sql = null;

                    if (txId == null)
                        sql = "SELECT * FROM DB2DBA.SESCTO_PIX WHERE STATUS = 'ATIVA' ORDER BY CRIACAO DESC ";
                    else
                        sql = "SELECT * FROM DB2DBA.SESCTO_PIX WHERE STATUS = 'ATIVA' AND TXID = @TXID ORDER BY CRIACAO DESC ";

                    var moviment = await connection.QueryAsync<SescTO_Pix>(sql, new { TXID = txId });

                    return moviment.AsList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        public async Task<SescTO_Pix> ObtemPixDb2(string txid)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = @$"SELECT * FROM DB2DBA.SESCTO_PIX WHERE TXID = '{txid}'";
                    var moviment = await connection.QuerySingleAsync<SescTO_Pix>(sql);
                    return moviment;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        public async Task<bool> VerificaPixPago(string txid)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = @$"SELECT * FROM DB2DBA.SESCTO_PIX WHERE TXID = '{txid}'";
                    var moviment = await connection.QuerySingleAsync<SescTO_Pix>(sql);
                    if (moviment != null)
                    {
                        if (moviment.STATUS == "CONCLUIDA")
                        {
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }
        public async Task<bool> IsPixEventoJaPago(string idclasse, string cdelement, int sqcobranca)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var cobranca = await connection.QueryFirstOrDefaultAsync<bool>(@"SELECT COUNT(*) FROM DB2DBA.SESCTO_PIX WHERE IDCLASSE = @idclasse AND CDELEMENT = @cdelement AND SQCOBRANCA = @sqcobranca AND STATUS = 'CONCLUIDA' ",
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
        public async Task<bool> SalvarPix(SescTO_Pix pix)
        {
            pix.PRODUCAO = _producao;

            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sql =
                    @"INSERT INTO DB2DBA.SESCTO_PIX (SQMATRIC, CDUOP, CDELEMENT, SQCOBRANCA, " +
                    "IDCLASSE, CRIACAO, EXPIRACAO, TXID, QRCODE, LOCATION, " +
                    "VALOR_ORIGINAL, VALOR_COBRANCA, JUROS, MULTA, DESCONTO, CHAVE, STATUS, SOLICITACAO, PRODUCAO, TIPO) VALUES" +
                    "(@SQMATRIC, @CDUOP, @CDELEMENT, @SQCOBRANCA, " +
                    "@IDCLASSE, @CRIACAO, @EXPIRACAO, @TXID, @QRCODE, @LOCATION, " +
                    "@VALOR_ORIGINAL, @VALOR_COBRANCA, @JUROS, @MULTA, @DESCONTO, @CHAVE, @STATUS, @SOLICITACAO, @PRODUCAO, @TIPO); ";
                var affectedRows = await connection.ExecuteAsync(sql, pix);
                if (affectedRows <= 0)
                {
                    return false;
                }
                else
                    return true;
            }
        }
        public async Task<bool> AtualizaStatusPix(string txid, string status)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                connection.Open();
                var sql = @"UPDATE DB2DBA.SESCTO_PIX SET STATUS = @status WHERE TXID = @txid ";
                var affectedRows = await connection.ExecuteAsync(sql, new { status = status, txid = txid });
                if (affectedRows <= 0)
                    return false;
                else
                    return true;
            }
        }

        public async Task<bool> AtualizaHorarioPixTransacaoes(BBClient bbClient)
        {
            using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = @$"SELECT TXID FROM SESCTO_PIX_TRANSACOES WHERE date(HORARIO) = '2024-01-24' ";
                    var listaTxId = await connection.QueryAsync<string>(sql);

                    foreach (var txId in listaTxId)
                    {
                        var response = await bbClient.ConsultarPix(txId);
                        var horario = response.Pix[0].Horario;

                        var sqls = @"UPDATE DB2DBA.SESCTO_PIX_TRANSACOES SET HORARIO = @horario WHERE TXID = @txid ";
                        var affectedRows = await connection.ExecuteAsync(sqls, new { horario = horario, txid = txId });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }         
            return true;
        }

        public async Task<bool> SalvarTransacaoPix(Pix pix, DB2Connection connection, DbTransaction transaction)
        {
            var sqlPix = @"UPDATE DB2DBA.SESCTO_PIX SET STATUS = @STATUS WHERE TXID = @TXID";
            await connection.ExecuteAsync(sqlPix, new { STATUS = "CONCLUIDA", TXID = pix.TxId }, transaction);

            var sql = @"INSERT INTO DB2DBA.SESCTO_PIX_TRANSACOES (ENDTOENDID, TXID, VALOR, HORARIO, " +
                                                             "CPF, NOME, INFORMACOES) VALUES" +
                                                             "(@ENDTOENDID, @TXID, @VALOR, @HORARIO, " +
                                                             "@CPF, @NOME, @INFORMACOES); ";
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                ENDTOENDID = pix.EndToEndId,
                TXID = pix.TxId,
                VALOR = pix.Valor,
                HORARIO = pix.Horario,
                CPF = pix.Pagador.Cpf != null ? pix.Pagador.Cpf : pix.Pagador.Cnpj,
                NOME = pix.Pagador.Nome,
                INFORMACOES = pix.InfoPagador
            }, transaction);

            if (affectedRows <= 0)
                return false;

            return true;
        }       
        public async Task<bool> AtualizarPixAtivos(string txId = null)
        {
            List<SescTO_Pix> pixAtivos = null;
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (txId == null)
                    pixAtivos = await this.ListarPixAtivos();
                else
                    pixAtivos = await this.ListarPixAtivos(txId);

                if (pixAtivos == null || pixAtivos.Count() == 0)
                    return false;

                var caixa = await _caixaRepository.ObterCaixaAberto();
                if (!(caixa is CACAIXA))
                {
                    caixa = await _caixaRepository.AbrirCaixa();
                    if (!(caixa is CACAIXA))
                        return false;
                }
                else
                {
                    using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                    {
                        connection.Open();
                        foreach (var pixAtivo in pixAtivos)
                        {
                            try
                            {
                                var apiPix = await _apiBancoBrasilService.BuscarPorTxidAsync(pixAtivo.TXID);                                
                                if (!apiPix.IsSuccess || apiPix.Content is null)
                                   continue;

                                var pixBB = apiPix.Content;
                                //var pixBB
                                //Atualiza cobran�as expiradas
                                DateTime expiracao = pixAtivo.CRIACAO.AddSeconds(pixAtivo.EXPIRACAO);

                                if (pixBB.Situacao == "EXPIRADA")
                                {
                                    var sql =
                                            @"UPDATE DB2DBA.SESCTO_PIX SET STATUS = @STATUS WHERE TXID = @TXID";
                                    var affectedRows = await connection.ExecuteAsync(sql, new { STATUS = "EXPIRADA", TXID = pixAtivo.TXID });
                                }
                                else
                                {
                                    //Atualiza mudan�a status
                                    {
                                        if (pixBB.Situacao != pixAtivo.STATUS)
                                            using (var transaction = await connection.BeginTransactionAsync())
                                            {
                                                try
                                                {
                                                    if (pixBB.Situacao == "CONCLUIDA")
                                                    {
                                                        //Pix Cobran�a Mensalidade
                                                        if (pixAtivo.TIPO == 1)
                                                        {
                                                            var cobrancaValor = new CobrancaValor
                                                            {
                                                                CDELEMENT = pixAtivo.CDELEMENT,
                                                                IDCLASSE = pixAtivo.IDCLASSE,
                                                                SQCOBRANCA = pixAtivo.SQCOBRANCA,
                                                                ValorOriginal = pixAtivo.VALOR_ORIGINAL,
                                                                ValorRecebido = Convert.ToDecimal(pixBB.ValorOriginal),
                                                                JurosMora = Convert.ToDecimal(pixAtivo.JUROS),
                                                                Multa = Convert.ToDecimal(pixAtivo.MULTA),
                                                                DescontoConcedido = Convert.ToDecimal(pixAtivo.DESCONTO)
                                                            };
                                                            await _cobrancaRepository.PagamentoCobranca(caixa, cobrancaValor, connection, transaction, 1055);
                                                        }
                                                        else if (pixAtivo.TIPO == 2) // Pix Recarga
                                                        {
                                                            await _cartaoRepository.RecargaCartaoSescPix(pixAtivo, pixBB);
                                                        }
                                                        else if (pixAtivo.TIPO == 3) // Pix Avulso - Corrida
                                                        {
                                                            await _siteRepository.BaixaInscricaoEvento(pixAtivo.TXID);
                                                        }
                                                    }
                                                    if (pixAtivo.TIPO != 2)
                                                    {
                                                        pixAtivo.STATUS = pixBB.Situacao;
                                                        //pixAtivo.STATUS = "CONCLUIDA";
                                                        var sql =
                                                                @"UPDATE DB2DBA.SESCTO_PIX SET STATUS = @STATUS WHERE TXID = @TXID";
                                                        var affectedRows = await connection.ExecuteAsync(sql, pixAtivo, transaction);
                                                        if (affectedRows <= 0)
                                                        {
                                                            await transaction.RollbackAsync();                                                           
                                                        }
                                                        /*
                                                        foreach (var transacao in pixBB.Pix)
                                                        {
                                                            sql =
                                                            @"INSERT INTO DB2DBA.SESCTO_PIX_TRANSACOES (ENDTOENDID, TXID, VALOR, HORARIO, " +
                                                            "CPF, NOME, INFORMACOES) VALUES" +
                                                            "(@ENDTOENDID, @TXID, @VALOR, @HORARIO, " +
                                                            "@CPF, @NOME, @INFORMACOES); ";
                                                            affectedRows = await connection.ExecuteAsync(sql, new
                                                            {
                                                                ENDTOENDID = transacao.EndToEndId,
                                                                TXID = transacao.TxId,
                                                                VALOR = transacao.Valor,
                                                                HORARIO = transacao.Horario,
                                                                CPF = transacao.Pagador.Cpf != null ? transacao.Pagador.Cpf : transacao.Pagador.Cnpj,
                                                                NOME = transacao.Pagador.Nome,
                                                                INFORMACOES = transacao.InfoPagador
                                                            }, transaction);

                                                            if (affectedRows <= 0)
                                                            {
                                                                await transaction.RollbackAsync();
                                                            }
                                                        }*/
                                                        await transaction.CommitAsync();
                                                    }
                                                }
                                                catch (Exception e)
                                                {                                                    
                                                    await transaction.RollbackAsync();
                                                }
                                            }
                                    }
                                }
                            }
                            catch (Exception e){ }
                        }
                    }                   
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _semaphoreSlim?.Release();
            }
        }
    }
}