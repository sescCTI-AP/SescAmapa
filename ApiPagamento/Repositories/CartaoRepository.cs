using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Pix;
using PagamentoApi.Models.Responses;
using PagamentoApi.Settings;

namespace PagamentoApi.Repositories
{
    public class CartaoRepository
    {
        private readonly IConfiguration configuration;

        public readonly CaixaSettings caixaConfiguration;
        private readonly CaixaRepository _caixaRepository;
        public CartaoRepository(IConfiguration configuration, CaixaRepository caixaRepository)
        {
            this.configuration = configuration;
            caixaConfiguration = configuration.GetSection("CaixaSettings").Get<CaixaSettings>();
            _caixaRepository = caixaRepository;
        }

        /// <summary>
        /// Obtem o saldo do cartão Sesc do cliente
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> ObterSaldo(int numcartao)
        {
            var statusCartao = await ConsultaStatusCartaoSesc(numcartao);

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var sql = "SELECT sldvlcart FROM SALDOCARTAO WHERE NUMCARTAO = @numcartao";
                var saldo = await connection.QuerySingleOrDefaultAsync<decimal>(sql, new { numcartao });
                return saldo;
            }
        }

        /// <summary>
        /// Retorna os dados de movimentação do Cartão Sesc
        /// </summary>
        /// <param name="numCartao">Numero do cartão PDV</param>
        /// <returns></returns>
        public async Task<List<MovCartao>> ObterMovimentacaoCartaoCliente(int numCartao)
        {
            //var movDictionary = new Dictionary<string, MovCartao>();
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = "SELECT H.DTMOVIMENT, H.HRMOVIMENT, H.VLPRODMOV, P.CDPRODUTO, P.DSPRODUTO FROM HSTMOVCART AS H " +
                        " INNER JOIN PRODUTOPDV AS P ON H.CDPRODUTO = P.CDPRODUTO WHERE H.NUMCARTAO = @numcartao ORDER BY H.DTMOVIMENT DESC, H.HRMOVIMENT DESC";
                    var moviment = await connection.QueryAsync<MovCartao>(sql, new { numcartao = numCartao });
                    return moviment.AsList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retorna a ultima movimentação do Cartão Sesc
        /// </summary>
        /// <param name="numCartao">Numero do cartão PDV</param>
        /// <returns></returns>
        public async Task<HSTMOVCART> ObterUltimaMovimentacaoCartaoCliente(int numCartao, CACAIXA cacaixa)
        {
            var movDictionary = new Dictionary<string, HSTMOVCART>();
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = "SELECT * FROM HSTMOVCART AS H INNER JOIN PRODUTOPDV P ON H.CDPRODUTO = P.CDPRODUTO WHERE H.CDPRODUTO = 0 AND H.NUMCARTAO = @numcartao AND DTMOVIMENT = CURRENT DATE ORDER BY H.DTMOVIMENT DESC, H.HRMOVIMENT DESC, H.SQMOVIMENT DESC";
                    var saldo = await connection.QueryAsync<HSTMOVCART, PRODUTOPDV, HSTMOVCART>(sql, (h, p) =>
                        {
                            h.PRODUTOPDV = p;
                            return h;
                        }, new
                        {
                            numcartao = numCartao,
                            SQCAIXA = cacaixa.SQCAIXA,
                            CDPESSOA = cacaixa.CDPESSOA
                        }, splitOn: "CDPRODUTO");
                    return saldo.FirstOrDefault();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        /// <summary>
        /// Solicita recarga de cartão sesc
        /// </summary>
        /// <param name="cartao"></param>
        /// <returns></returns>
        public async Task<string> RecargaCartaoCielo(int numCartao, decimal valorDeposito, HSTMOVCART ultimaMovimentacao, CXDEPRETPDV ultimoCxDepret, CACAIXA caixa, PagamentoCielo pagamentoCielo)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "INSERT INTO SESCTO_CIELO (IDCLASSE, CDELEMENT, SQCOBRANC, MERCHANTORDER, CARDNUMBER, BRAND, PROOFOFSALE, TID, AUTHORIZATIONCODE, " +
                            " PAYMENTID, TIPO, VALOR, PARCELAS, DTOPERACAO, CAIXA, CDPESSOA, SQDEPRET, CANCELADO) " +
                            " VALUES (@IDCLASSE, @CDELEMENT, @SQCOBRANC, @MERCHANTORDER, @CARDNUMBER, @BRAND, @PROOFOFSALE, @TID, @AUTHORIZATIONCODE, " +
                            " @PAYMENTID, @TIPO, @VALOR, @PARCELAS, @DTOPERACAO, @CAIXA, @CDPESSOA, @SQDEPRET, @CANCELADO) ";

                        await connection.ExecuteAsync(
                            sql,
                            pagamentoCielo,
                            transaction
                        );

                        // Recupera total atual do cartão
                        var cartCred = await this.ConsultaTotalRecargaCartao(numCartao);

                        if (cartCred is CARTCRED)
                        {
                            sql = "UPDATE DB2DBA.CARTCRED SET CDPRODUTO = @CDPRODUTO, QTDPRODCRE = @QTDPRODCRE, VALPRODCRE = @VALPRODCRE, QTDPRODBLO = @QTDPRODBLO," +
                                "VBATIVO = @VBATIVO, VALPRODBLO = @VALPRODBLO, DTATU = @DTATU, HRATU = @HRATU, LGATU = @LGATU WHERE NUMCARTAO = @NUMCARTAO;";
                            cartCred.VALPRODCRE += valorDeposito;
                        }
                        else
                        {
                            cartCred = new CARTCRED();
                            cartCred.NUMCARTAO = numCartao;
                            cartCred.VALPRODCRE = valorDeposito;
                            sql = "INSERT INTO CARTCRED " +
                                "(NUMCARTAO, CDPRODUTO, QTDPRODCRE, VALPRODCRE, QTDPRODBLO, VBATIVO, VALPRODBLO, DTATU, HRATU, LGATU)" +
                                "VALUES (@NUMCARTAO, @CDPRODUTO, @QTDPRODCRE, @VALPRODCRE, @QTDPRODBLO, @VBATIVO, @VALPRODBLO, @DTATU, @HRATU, @LGATU);";
                        }

                        cartCred.CDPRODUTO = 0;
                        cartCred.QTDPRODCRE = new decimal?(1);
                        cartCred.QTDPRODBLO = new decimal?(0m);
                        cartCred.VALPRODBLO = new decimal?(0m);
                        cartCred.VBATIVO = 1;
                        cartCred.DTATU = DateTime.Now.Date;
                        cartCred.HRATU = DateTime.Now.TimeOfDay;
                        cartCred.LGATU = caixaConfiguration.CdPessoa.ToString();

                        var affectedRows = await connection.ExecuteAsync(sql, cartCred, transaction);

                        HSTMOVCART movCart = new HSTMOVCART();
                        movCart.CDPESSOA = caixaConfiguration.CdPessoa;
                        movCart.SQCAIXA = caixa.SQCAIXA;
                        movCart.CDPRODUTO = 0;
                        movCart.NUMCARTAO = numCartao;
                        movCart.SQMOVIMENT = (short)((ultimaMovimentacao != null) ? (ultimaMovimentacao.SQMOVIMENT + 1) : 1);
                        movCart.VBCREDEB = 0;
                        movCart.DTMOVIMENT = DateTime.Now.Date;
                        movCart.HRMOVIMENT = DateTime.Now.TimeOfDay;
                        movCart.TPMOVIMENT = 0;
                        movCart.OBSMOVIMEN = "Aquisição de Créditos (Recarga on-line via e-commerce)";
                        movCart.QTDPRODMOV = 0m;
                        movCart.VLPRODMOV = valorDeposito;
                        movCart.DTATU = DateTime.Now.Date;
                        movCart.HRATU = DateTime.Now.TimeOfDay;
                        movCart.LGATU = caixaConfiguration.CdPessoa.ToString();
                        movCart.IDCHECKOUT = 0;
                        movCart.IDUSUARIO = "XPTO";

                        sql = "INSERT INTO HSTMOVCART " +
                            "(CDPRODUTO, DTMOVIMENT, NUMCARTAO, SQMOVIMENT, VBCREDEB, HRMOVIMENT, TPMOVIMENT, QTDPRODMOV, VLPRODMOV, OBSMOVIMEN, DTATU, HRATU, LGATU, " +
                            "IDCHECKOUT, IDUSUARIO, SQCAIXA, IDHSTMOVCART, CDPESSOA) " +
                            " VALUES(@CDPRODUTO, @DTMOVIMENT, @NUMCARTAO, @SQMOVIMENT, @VBCREDEB, @HRMOVIMENT, @TPMOVIMENT, @QTDPRODMOV, @VLPRODMOV, @OBSMOVIMEN, @DTATU, @HRATU, @LGATU, " +
                            "@IDCHECKOUT, @IDUSUARIO, @SQCAIXA, (select MAX(IDHSTMOVCART) from HSTMOVCART) + 1, @CDPESSOA)";
                        affectedRows = await connection.ExecuteAsync(sql, movCart, transaction);
                        //await this._hstMovCartService.SalvarMovimentacaoCartao(movCart);

                        CXDEPRETPDV cxDepret = new CXDEPRETPDV();
                        cxDepret.TPDEPRET = 0;
                        cxDepret.CDPESSOA = caixaConfiguration.CdPessoa;
                        cxDepret.SQCAIXA = caixa.SQCAIXA;
                        cxDepret.SQDEPRET = (short)((ultimoCxDepret == null) ? 1 : (ultimoCxDepret.SQDEPRET + 1));
                        cxDepret.VLDEPRET = valorDeposito;
                        cxDepret.DTDEPRET = DateTime.Now.Date;
                        cxDepret.HRDEPRET = DateTime.Now.TimeOfDay;
                        cxDepret.STDEPRET = 0;
                        cxDepret.NUMCARTAO = numCartao;
                        cxDepret.CDMOEDAPGT = 3;

                        sql = "INSERT INTO CXDEPRETPDV " +
                            "(TPDEPRET, SQDEPRET, SQCAIXA, VLDEPRET, VLENCARGOS, DTDEPRET, HRDEPRET, STDEPRET, CDMOEDAPGT, NUMCARTAO, CDPESSOA)" +
                            "VALUES (@TPDEPRET, @SQDEPRET, @SQCAIXA, @VLDEPRET, @VLENCARGOS, @DTDEPRET, @HRDEPRET, @STDEPRET, @CDMOEDAPGT, @NUMCARTAO, @CDPESSOA);";

                        affectedRows = await connection.ExecuteAsync(sql, cxDepret, transaction);
                        //await this._cxdePretPdvService.SalvarCxdePretPdv(cxDepret);

                        //await this._cartCredService.AtualizarCartao(cartCred);
                        //pedido.Status = new int?(StatusPedidoEnum.Pago.GetHashCode());
                        //this._pedidoService.AtualizarPedido(pedido, null, TipoMovimentoEnum.Notificacao);

                        // if it was successful, commit the transaction
                        await transaction.CommitAsync();
                        return "";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine(e);
                        return $"Erro ao salvar recarga de saldo: {e.Message}";
                        //throw;
                    }
                }
            }
        }

        /// <summary>
        /// Solicita recarga de cartão sesc
        /// </summary>
        /// <param name="cartao"></param>
        /// <returns></returns>
        public async Task<string> RecargaCartao(int numCartao, decimal valorDeposito, HSTMOVCART ultimaMovimentacao, CXDEPRETPDV ultimoCxDepret, CACAIXA caixa, int cdmoeda = 1065, bool caixaTef = false)
        {
            var statusCartao = await ConsultaStatusCartaoSesc(numCartao);

            if (statusCartao)
            {
                var cdpessoa = caixaConfiguration.CdPessoa;
                if (caixaTef == true)
                    cdpessoa = caixaConfiguration.CdPessoaTef;

                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            HSTMOVCART movCart = new HSTMOVCART();
                            movCart.CDPESSOA = cdpessoa;
                            movCart.SQCAIXA = caixa.SQCAIXA;
                            movCart.CDPRODUTO = 0;
                            movCart.NUMCARTAO = numCartao;
                            movCart.SQMOVIMENT = (short)((ultimaMovimentacao != null) ? (ultimaMovimentacao.SQMOVIMENT + 1) : 1);
                            movCart.VBCREDEB = 0;
                            movCart.DTMOVIMENT = DateTime.Now.Date;
                            movCart.HRMOVIMENT = DateTime.Now.TimeOfDay;
                            movCart.TPMOVIMENT = 0;
                            movCart.OBSMOVIMEN = "Aquisição de Créditos (Recarga on-line via e-commerce)";
                            movCart.QTDPRODMOV = 0m;
                            movCart.VLPRODMOV = valorDeposito;
                            movCart.DTATU = DateTime.Now.Date;
                            movCart.HRATU = DateTime.Now.TimeOfDay;
                            movCart.LGATU = cdpessoa.ToString();
                            movCart.IDCHECKOUT = 0;

                            var sql = "INSERT INTO HSTMOVCART " +
                                "(CDPRODUTO, DTMOVIMENT, NUMCARTAO, SQMOVIMENT, VBCREDEB, HRMOVIMENT, TPMOVIMENT, QTDPRODMOV, VLPRODMOV, OBSMOVIMEN, DTATU, HRATU, LGATU, " +
                                "IDCHECKOUT, IDUSUARIO, SQCAIXA, IDHSTMOVCART, CDPESSOA) " +
                                " VALUES(@CDPRODUTO, @DTMOVIMENT, @NUMCARTAO, @SQMOVIMENT, @VBCREDEB, @HRMOVIMENT, @TPMOVIMENT, @QTDPRODMOV, @VLPRODMOV, @OBSMOVIMEN, @DTATU, @HRATU, @LGATU, " +
                                "@IDCHECKOUT, @IDUSUARIO, @SQCAIXA, @IDHSTMOVCART, @CDPESSOA)";
                            var affectedRows = await connection.ExecuteAsync(sql, movCart, transaction);
                            //await this._hstMovCartService.SalvarMovimentacaoCartao(movCart);

                            CXDEPRETPDV cxDepret = new CXDEPRETPDV();
                            cxDepret.TPDEPRET = 0;
                            cxDepret.CDPESSOA = cdpessoa;
                            cxDepret.SQCAIXA = caixa.SQCAIXA;
                            cxDepret.SQDEPRET = (short)((ultimoCxDepret == null) ? 1 : (ultimoCxDepret.SQDEPRET + 1));
                            cxDepret.VLDEPRET = valorDeposito;
                            cxDepret.DTDEPRET = DateTime.Now.Date;
                            cxDepret.HRDEPRET = DateTime.Now.TimeOfDay;
                            cxDepret.STDEPRET = 0;
                            cxDepret.NUMCARTAO = numCartao;
                            cxDepret.CDMOEDAPGT = cdmoeda;

                            sql = "INSERT INTO CXDEPRETPDV " +
                                "(TPDEPRET, SQDEPRET, SQCAIXA, VLDEPRET, VLENCARGOS, DTDEPRET, HRDEPRET, STDEPRET, CDMOEDAPGT, NUMCARTAO, CDPESSOA)" +
                                "VALUES (@TPDEPRET, @SQDEPRET, @SQCAIXA, @VLDEPRET, @VLENCARGOS, @DTDEPRET, @HRDEPRET, @STDEPRET, @CDMOEDAPGT, @NUMCARTAO, @CDPESSOA);";

                            affectedRows = await connection.ExecuteAsync(sql, cxDepret, transaction);
                            //await this._cxdePretPdvService.SalvarCxdePretPdv(cxDepret);

                            // Recupera total atual do cartão
                            var cartCred = await this.ConsultaTotalRecargaCartao(numCartao);
                            if (cartCred is CARTCRED)
                            {
                                sql = "UPDATE DB2DBA.CARTCRED SET CDPRODUTO = @CDPRODUTO, QTDPRODCRE = @QTDPRODCRE, VALPRODCRE = @VALPRODCRE, QTDPRODBLO = @QTDPRODBLO," +
                                    "VBATIVO = @VBATIVO, VALPRODBLO = @VALPRODBLO, DTATU = @DTATU, HRATU = @HRATU, LGATU = @LGATU WHERE NUMCARTAO = @NUMCARTAO;";
                            }
                            else
                            {
                                cartCred = new CARTCRED();
                                cartCred.NUMCARTAO = numCartao;
                                sql = "INSERT INTO CARTCRED " +
                                    "(NUMCARTAO, CDPRODUTO, QTDPRODCRE, VALPRODCRE, QTDPRODBLO, VBATIVO, VALPRODBLO, DTATU, HRATU, LGATU)" +
                                    "VALUES (@NUMCARTAO, @CDPRODUTO, @QTDPRODCRE, @VALPRODCRE, @QTDPRODBLO, @VBATIVO, @VALPRODBLO, @DTATU, @HRATU, @LGATU);";
                            }

                            cartCred.CDPRODUTO = 0;
                            cartCred.QTDPRODCRE = new decimal?(1);
                            cartCred.VALPRODCRE = cartCred.VALPRODCRE == null ? valorDeposito : cartCred.VALPRODCRE + valorDeposito;
                            cartCred.QTDPRODBLO = new decimal?(0m);
                            cartCred.VALPRODBLO = new decimal?(0m);
                            cartCred.VBATIVO = 1;
                            cartCred.DTATU = DateTime.Now.Date;
                            cartCred.HRATU = DateTime.Now.TimeOfDay;
                            cartCred.LGATU = cdpessoa.ToString();

                            affectedRows = await connection.ExecuteAsync(sql, cartCred, transaction);
                             
                            //await this._cartCredService.AtualizarCartao(cartCred);
                            //pedido.Status = new int?(StatusPedidoEnum.Pago.GetHashCode());
                            //this._pedidoService.AtualizarPedido(pedido, null, TipoMovimentoEnum.Notificacao);

                            // if it was successful, commit the transaction
                            await transaction.CommitAsync();
                            return "";
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(e);
                            return $"Erro ao salvar recarga de saldo: {e.Message}";
                            //throw;
                        }
                    }
                }
            }
            return $"Não foi possível efetuar sua recarga no momento. Por favor entre em contato com a central de atendimento.";
        }


        /// <summary>
        /// Solicita recarga de cartão sesc por pix
        /// </summary>
        /// <param name="cartao"></param>
        /// <returns></returns>
        public async Task<string> RecargaCartaoSescPix(SescTO_Pix pixAtivo, TransacaoPixResponse pixBB)
        {
            if (pixAtivo.TIPO != 2)
                return "Não existe um caixa aberto";

            if (pixAtivo.STATUS == "CONCLUIDA")
                return "Transação já processada";

            var caixa = await _caixaRepository.ObterCaixaAberto();
            if (!(caixa is CACAIXA))
                return "Não existe um caixa aberto";

            var numCartao = Convert.ToInt32(pixAtivo.CDELEMENT);
            var valorDeposito = pixAtivo.VALOR_ORIGINAL;
            var ultimoCxDepret = await _caixaRepository.ObterCaixaDePret(caixa);
            var ultimaMovimentacao = await ObterUltimaMovimentacaoCartaoCliente(numCartao, caixa);
            var statusCartao = await ConsultaStatusCartaoSesc(numCartao);

            if (statusCartao)
            {
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            HSTMOVCART movCart = new HSTMOVCART();
                            movCart.CDPESSOA = caixa.CDPESSOA;
                            movCart.SQCAIXA = caixa.SQCAIXA;
                            movCart.CDPRODUTO = 0;
                            movCart.NUMCARTAO = numCartao;
                            movCart.SQMOVIMENT = (short)((ultimaMovimentacao != null) ? (ultimaMovimentacao.SQMOVIMENT + 1) : 1);
                            movCart.VBCREDEB = 0;
                            movCart.DTMOVIMENT = DateTime.Now.Date;
                            movCart.HRMOVIMENT = DateTime.Now.TimeOfDay;
                            movCart.TPMOVIMENT = 0;
                            movCart.OBSMOVIMEN = "Aquisição de Créditos (Recarga on-line via e-commerce)";
                            movCart.QTDPRODMOV = 0m;
                            movCart.VLPRODMOV = valorDeposito;
                            movCart.DTATU = DateTime.Now.Date;
                            movCart.HRATU = DateTime.Now.TimeOfDay;
                            movCart.LGATU = caixaConfiguration.CdPessoa.ToString();
                            movCart.IDCHECKOUT = 0;

                            var sql = "INSERT INTO HSTMOVCART " +
                                "(CDPRODUTO, DTMOVIMENT, NUMCARTAO, SQMOVIMENT, VBCREDEB, HRMOVIMENT, TPMOVIMENT, QTDPRODMOV, VLPRODMOV, OBSMOVIMEN, DTATU, HRATU, LGATU, " +
                                "IDCHECKOUT, IDUSUARIO, SQCAIXA, IDHSTMOVCART, CDPESSOA) " +
                                " VALUES(@CDPRODUTO, @DTMOVIMENT, @NUMCARTAO, @SQMOVIMENT, @VBCREDEB, @HRMOVIMENT, @TPMOVIMENT, @QTDPRODMOV, @VLPRODMOV, @OBSMOVIMEN, @DTATU, @HRATU, @LGATU, " +
                                "@IDCHECKOUT, @IDUSUARIO, @SQCAIXA, @IDHSTMOVCART, @CDPESSOA)";
                            var affectedRows = await connection.ExecuteAsync(sql, movCart, transaction);

                            //await this._hstMovCartService.SalvarMovimentacaoCartao(movCart);

                            CXDEPRETPDV cxDepret = new CXDEPRETPDV();
                            cxDepret.TPDEPRET = 0;
                            cxDepret.CDPESSOA = caixa.CDPESSOA;
                            cxDepret.SQCAIXA = caixa.SQCAIXA;
                            cxDepret.SQDEPRET = (short)((ultimoCxDepret == null) ? 1 : (ultimoCxDepret.SQDEPRET + 1));
                            cxDepret.VLDEPRET = valorDeposito;
                            cxDepret.DTDEPRET = DateTime.Now.Date;
                            cxDepret.HRDEPRET = DateTime.Now.TimeOfDay;
                            cxDepret.STDEPRET = 0;
                            cxDepret.NUMCARTAO = numCartao;
                            cxDepret.CDMOEDAPGT = 1055;

                            sql = "INSERT INTO CXDEPRETPDV " +
                                "(TPDEPRET, SQDEPRET, SQCAIXA, VLDEPRET, VLENCARGOS, DTDEPRET, HRDEPRET, STDEPRET, CDMOEDAPGT, NUMCARTAO, CDPESSOA)" +
                                "VALUES (@TPDEPRET, @SQDEPRET, @SQCAIXA, @VLDEPRET, @VLENCARGOS, @DTDEPRET, @HRDEPRET, @STDEPRET, @CDMOEDAPGT, @NUMCARTAO, @CDPESSOA);";

                            affectedRows = await connection.ExecuteAsync(sql, cxDepret, transaction);
                            //await this._cxdePretPdvService.SalvarCxdePretPdv(cxDepret);

                            // Recupera total atual do cartão
                            var cartCred = await this.ConsultaTotalRecargaCartao(numCartao);
                            if (cartCred is CARTCRED)
                            {
                                sql = "UPDATE DB2DBA.CARTCRED SET CDPRODUTO = @CDPRODUTO, QTDPRODCRE = @QTDPRODCRE, VALPRODCRE = @VALPRODCRE, QTDPRODBLO = @QTDPRODBLO," +
                                    "VBATIVO = @VBATIVO, VALPRODBLO = @VALPRODBLO, DTATU = @DTATU, HRATU = @HRATU, LGATU = @LGATU WHERE NUMCARTAO = @NUMCARTAO;";
                            }
                            else
                            {
                                cartCred = new CARTCRED();
                                cartCred.NUMCARTAO = numCartao;
                                cartCred.VALPRODCRE = 0;
                                sql = "INSERT INTO CARTCRED " +
                                    "(NUMCARTAO, CDPRODUTO, QTDPRODCRE, VALPRODCRE, QTDPRODBLO, VBATIVO, VALPRODBLO, DTATU, HRATU, LGATU)" +
                                    "VALUES (@NUMCARTAO, @CDPRODUTO, @QTDPRODCRE, @VALPRODCRE, @QTDPRODBLO, @VBATIVO, @VALPRODBLO, @DTATU, @HRATU, @LGATU);";
                            }

                            cartCred.CDPRODUTO = 0;
                            cartCred.QTDPRODCRE = new decimal?(1);
                            cartCred.VALPRODCRE = cartCred.VALPRODCRE == null ? valorDeposito : cartCred.VALPRODCRE + valorDeposito;
                            cartCred.QTDPRODBLO = new decimal?(0m);
                            cartCred.VALPRODBLO = new decimal?(0m);
                            cartCred.VBATIVO = 1;
                            cartCred.DTATU = DateTime.Now.Date;
                            cartCred.HRATU = DateTime.Now.TimeOfDay;
                            cartCred.LGATU = caixaConfiguration.CdPessoa.ToString();

                            affectedRows = await connection.ExecuteAsync(sql, cartCred, transaction);

                            //await this._cartCredService.AtualizarCartao(cartCred);
                            //pedido.Status = new int?(StatusPedidoEnum.Pago.GetHashCode());
                            //this._pedidoService.AtualizarPedido(pedido, null, TipoMovimentoEnum.Notificacao);

                            // if it was successful, commit the transaction

                            #region Atualiza Pix Recarga
                            //pixAtivo.STATUS = "CONCLUIDA";
                            pixAtivo.STATUS = pixBB.Situacao;
                            pixAtivo.SQCOBRANCA = Convert.ToInt32($"{cxDepret.SQCAIXA}{cxDepret.SQDEPRET.ToString().PadLeft(5, '0')}");
                            var atualizaPix = @"UPDATE DB2DBA.SESCTO_PIX SET STATUS = @STATUS, SQCOBRANCA = @SQCOBRANCA WHERE TXID = @TXID";

                            var affectedRowsPix = await connection.ExecuteAsync(atualizaPix, pixAtivo, transaction);
                            if (affectedRowsPix <= 0)
                            {
                                await transaction.RollbackAsync();
                                return "Erro ao atualizar PIX";
                            }
                            /*
                            foreach (var transacao in pixBB.Pix)
                            {
                                sql =
                                @"INSERT INTO DB2DBA.SESCTO_PIX_TRANSACOES (ENDTOENDID, TXID, VALOR, HORARIO, " +
                                "CPF, NOME, INFORMACOES) VALUES" +
                                "(@ENDTOENDID, @TXID, @VALOR, @HORARIO, " +
                                "@CPF, @NOME, @INFORMACOES); ";
                                affectedRowsPix = await connection.ExecuteAsync(sql, new
                                {
                                    ENDTOENDID = transacao.EndToEndId,
                                    TXID = transacao.TxId,
                                    VALOR = transacao.Valor,
                                    HORARIO = transacao.Horario,
                                    CPF = transacao.Pagador.Cpf != null ? transacao.Pagador.Cpf : transacao.Pagador.Cnpj,
                                    NOME = transacao.Pagador.Nome,
                                    INFORMACOES = transacao.InfoPagador
                                }, transaction);

                                if (affectedRowsPix <= 0)
                                {
                                    await transaction.RollbackAsync();
                                    return "Erro ao atualizar PIX";
                                }
                            }*/
                            #endregion

                            await transaction.CommitAsync();
                            return "";
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(e);
                            return $"Erro ao salvar recarga de saldo: {e.Message}";
                            //throw;
                        }
                    }
                }
            }
            return $"Não foi possível efetuar sua recarga no momento.";
        }




        /// <summary>
        /// Insere saldo no cartão sesc
        /// </summary>
        /// <param name="cartao"></param>
        /// <returns></returns>
        public async Task<bool> InserirSaldoCartao([FromBody] RecargaValor recarga)
        {
            var cacaixa = await _caixaRepository.ObterCaixaAberto();
            if (!(cacaixa is CACAIXA))
                return false;

            var caixadepret = await _caixaRepository.ObterCaixaDePret(cacaixa);

            var ultimaMovimentacao = await ObterUltimaMovimentacaoCartaoCliente(recarga.NumCartao, cacaixa);

            var result = await RecargaCartao(recarga.NumCartao, recarga.Valor, ultimaMovimentacao, caixadepret, cacaixa);

            if (result == "")
                return true;
            return false;
        }

        public async Task<CARTCRED> ConsultaTotalRecargaCartao(int numcartao)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = "SELECT * FROM CARTCRED WHERE NUMCARTAO = @numcartao ";
                    var saldo = await connection.QueryFirstOrDefaultAsync<CARTCRED>(sql, new { numcartao = numcartao });
                    return saldo;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task<bool> ConsultaStatusCartaoSesc(int numcartao)
        {
            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                try
                {
                    await connection.OpenAsync();
                    var sql = "SELECT * FROM CARTAO WHERE NUMCARTAO = @numcartao ";
                    var cartao = await connection.QueryFirstOrDefaultAsync<CARTAO>(sql, new { numcartao = numcartao });
                    if (cartao.TPCONTRL == 1)
                    {
                        return true;
                    }
                    else
                    {
                        var atualizaTipoCartao = @"UPDATE DB2DBA.CARTAO SET TPCONTRL = 1 WHERE NUMCARTAO = @numcartao";
                        var affectedRows = await connection.ExecuteAsync(atualizaTipoCartao, new { numcartao = numcartao });
                        if (affectedRows <= 0)
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
                    //throw;
                }
            }
        }

        public async Task<CLIENTELA> ObterClientePorNumCartao(string numcartao)
        {

            using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
            {
                await connection.OpenAsync();
                var clientelaDictionary = new Dictionary<string, CLIENTELA>();
                var cliente = await connection.QueryFirstOrDefaultAsync<CLIENTELA>(
                    @"SELECT CDUOP, SQMATRIC, NUDV, CDCATEGORI, DTVENCTO, NMCLIENTE, DTNASCIMEN, STMATRIC, NUCPF, NUMCARTAO FROM CLIENTELA WHERE NUMCARTAO = @NUMCARTAO",
                    new
                    {
                        numcartao,
                    });
                return cliente;
            }
        }
    }
}