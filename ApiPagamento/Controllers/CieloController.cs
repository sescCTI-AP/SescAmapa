using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagamentoApi.ApiAutenticacoes;
using PagamentoApi.Clients;
using PagamentoApi.DTOs;
using PagamentoApi.Models;
using PagamentoApi.Models.Cielo;
using PagamentoApi.Models.HabilitaTransacoes;
using PagamentoApi.Repositories;

namespace PagamentoApi.Controllers
{
    
    [Route("v1/[controller]")]
    [ApiController]
    public class CieloController : ControllerBase
    {
        [HttpPost("pagar-cobranca")]
        [Authorize]
        public async Task<dynamic> PagarCobranca([FromServices] CieloClient cieloClient, [FromServices] CobrancaRepository cobrancaRepository, [FromServices] CaixaRepository caixaRepository, [FromBody] Transacao transacao)
        {
            if (!HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));

            var cobranca = await cobrancaRepository.ObterCobrancaCompleta(transacao.CobrancaValor.IDCLASSE, transacao.CobrancaValor.CDELEMENT, transacao.CobrancaValor.SQCOBRANCA);
            if (cobranca.STRECEBIDO != 0)
            {
                return BadRequest("Cobranca paga ou cancelada anteriormente.");
            }

            transacao.MerchantOrderId = $"{transacao.CobrancaValor.IDCLASSE.Trim()}-{transacao.CobrancaValor.CDELEMENT.Trim()}-{transacao.CobrancaValor.SQCOBRANCA}"; //Numero do pedido

            var caixa = await caixaRepository.ObterCaixaAberto();
            if (!(caixa is CACAIXA))
            {
                caixa = await caixaRepository.AbrirCaixa();
                if (!(caixa is CACAIXA))
                    return BadRequest("Não existe um caixa aberto");
            }

            //validar valor enviado
            transacao.Payment.Capture = true;
            var cobrancaValor = await cobrancaRepository.ObterCobranca(transacao.CobrancaValor.IDCLASSE, transacao.CobrancaValor.CDELEMENT, transacao.CobrancaValor.SQCOBRANCA);
            cobrancaValor.ValorRecebido = transacao.CobrancaValor.ValorRecebido;
            cobrancaValor.ValorOriginal = transacao.CobrancaValor.ValorOriginal;
            cobrancaValor.DescontoConcedido = transacao.CobrancaValor.DescontoConcedido;
            // if (cobrancaValor.ValorRecebido != transacao.Payment.Amount)
            //     return BadRequest("Valor divergente");

            //converte valor de decimal para inteiro usado pela cielo
            transacao.Payment.Amount = decimal.ToInt32(transacao.Payment.Amount * 100);

            //efetuar cobranca cielo
            var autorizacao = await cieloClient.Autorizacao(transacao);

            if (autorizacao.Payment != null && (autorizacao.Payment.ReturnCode.Equals("00") || autorizacao.Payment.ReturnCode.Equals("4") || autorizacao.Payment.ReturnCode.Equals("6")))
            {
                try
                {
                    var pagamentoCielo = new PagamentoCielo
                    {
                        IDCLASSE = transacao.CobrancaValor.IDCLASSE,
                        CDELEMENT = transacao.CobrancaValor.CDELEMENT,
                        SQCOBRANC = transacao.CobrancaValor.SQCOBRANCA,
                        MERCHANTORDER = autorizacao.MerchantOrderId,
                        PROOFOFSALE = autorizacao.Payment.ProofOfSale,
                        TID = autorizacao.Payment.Tid,
                        AUTHORIZATIONCODE = autorizacao.Payment.AuthorizationCode,
                        PAYMENTID = autorizacao.Payment.PaymentId.ToString(),
                        TIPO = autorizacao.Payment.Type,
                        VALOR = Convert.ToDecimal(autorizacao.Payment.Amount) / 100,
                        PARCELAS = autorizacao.Payment.Installments,
                        DTOPERACAO = autorizacao.Payment.ReceivedDate,
                        BRAND = autorizacao.Payment.CreditCard.Brand,
                        CARDNUMBER = autorizacao.Payment.CreditCard.CardNumber,
                        CAIXA = caixa.SQCAIXA,
                        CDPESSOA = caixa.CDPESSOA,
                        SQDEPRET = 0,
                        CANCELADO = 0
                    };
                    var retornoCielo = await cobrancaRepository.PagamentoCobrancaCielo(pagamentoCielo, caixa, cobrancaValor);
                    if (retornoCielo != "")
                    { //cancelar transacao
                        var retornoCancelamento = await cieloClient.Cancelar(autorizacao.Payment.PaymentId, autorizacao.Payment.Amount);
                        return BadRequest(retornoCielo);
                    }
                    return Ok("Cobrança paga com sucesso.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    var retornoCancelamento = await cieloClient.Cancelar(autorizacao.Payment.PaymentId, autorizacao.Payment.Amount);
                }
            }
            return BadRequest(autorizacao);
        }

        [HttpPost("cancela")]
        [Authorize]
        public async Task<dynamic> Cancelar([FromServices] CieloClient cieloClient, [FromBody] ReturnPaymentTransacao payment)
        {
            return await cieloClient.Cancelar(payment.PaymentId, payment.Amount);
        }

        [HttpPost("dados-compra")]
        [Authorize]
        public async Task<dynamic> Compra([FromServices] CieloClient cieloClient, [FromBody] ReturnPaymentTransacao payment)
        {
            return await cieloClient.GetCompra(payment.PaymentId);
        }

        [HttpGet("dados-compra-tid/{tid}")]
        public async Task<dynamic> CompraPorTid([FromServices] CieloClient cieloClient, [FromServices] IApiAutenticacao apiAutenticacao, string tid)
        {
            if (string.IsNullOrEmpty(tid))
                return false;

            string? apiKey = Request.Headers[Constants.ApiChaveHeaderNome];

            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest();

            bool isValido = apiAutenticacao.IsValidoChaveApi(apiKey);

            if (!isValido)
                return Unauthorized();

            return await cieloClient.GetCompraPorTid(tid);
        }

        [HttpGet("compras-pedido/{merchantOrderId}")]
        [Authorize]
        public async Task<dynamic> ObterComprasPorPedido([FromServices] CieloClient cieloClient, string merchantOrderId)
        {
            return await cieloClient.GetCompraPorPedido(merchantOrderId);
        }

        [HttpPost("recarga")]
        [Authorize]
        public async Task<dynamic> Recarregar([FromServices] CieloClient cieloClient, [FromServices] CartaoRepository cartaoRepository, [FromServices] CaixaRepository caixaRepository, [FromServices] CobrancaRepository cobrancaRepository, [FromBody] Recarga recarga)
        {   
            if(recarga.Valor < 2 || recarga.Valor > 150)
                return BadRequest("Desculpe, não foi possível prosseguir. O valor minímo da recarga é R$ 2,00 e o máximo é R$ 150,00");

            if (!HabilitaTransacao.IsHorarioDeGeracao())
                return Ok(new ResponseGenericoResult(false, HabilitaTransacao.messagem, null));

            if (recarga.NumCartao <= 0)
            {
                return BadRequest("Cliente não possui Cartão gerado");
            }
            var caixa = await caixaRepository.ObterCaixaAberto();
            if (!(caixa is CACAIXA))
            {
                caixa = await caixaRepository.AbrirCaixa();
                if (!(caixa is CACAIXA))
                    return BadRequest("Não existe um caixa aberto");
            }
            var ultimaMovimentacao = await cartaoRepository.ObterUltimaMovimentacaoCartaoCliente(recarga.NumCartao, caixa);
            var sqUltimaMov = ultimaMovimentacao != null ? ultimaMovimentacao.SQMOVIMENT : 0;
            var dataAtual = DateTime.Today;
            var transacao = new Transacao
            {
                MerchantOrderId = $"CART-{recarga.NumCartao}-{dataAtual.Year}{dataAtual.Month}{dataAtual.Day}-{sqUltimaMov + 1}",
                Payment = recarga.Payment,
                Customer = recarga.Customer
            }; //Numero do pedido
            transacao.Payment.Capture = true;

            //converte valor de decimal para inteiro usado pela cielo
            transacao.Payment.Amount = decimal.ToInt32(transacao.Payment.Amount * 100);

            //efetuar cobranca cielo
            var autorizacao = await cieloClient.Autorizacao(transacao);

            if (autorizacao.Payment != null && (autorizacao.Payment.ReturnCode.Equals("00") || autorizacao.Payment.ReturnCode.Equals("4") || autorizacao.Payment.ReturnCode.Equals("6")))
            {
                try
                {
                    var caixadepret = await caixaRepository.ObterCaixaDePret(caixa);
                    var pagamentoCielo = new PagamentoCielo
                    {
                        IDCLASSE = "CART",
                        CDELEMENT = recarga.NumCartao.ToString(),
                        SQCOBRANC = sqUltimaMov + 1,
                        MERCHANTORDER = autorizacao.MerchantOrderId,
                        PROOFOFSALE = autorizacao.Payment.ProofOfSale,
                        TID = autorizacao.Payment.Tid,
                        AUTHORIZATIONCODE = autorizacao.Payment.AuthorizationCode,
                        PAYMENTID = autorizacao.Payment.PaymentId.ToString(),
                        TIPO = autorizacao.Payment.Type,
                        VALOR = Convert.ToDecimal(autorizacao.Payment.Amount) / 100,
                        DTOPERACAO = autorizacao.Payment.ReceivedDate,
                        BRAND = autorizacao.Payment.CreditCard.Brand,
                        CARDNUMBER = autorizacao.Payment.CreditCard.CardNumber,
                        PARCELAS = autorizacao.Payment.Installments,
                        CAIXA = caixa.SQCAIXA,
                        CDPESSOA = caixa.CDPESSOA,
                        SQDEPRET = caixadepret != null ? caixadepret.SQDEPRET : 1,
                        CANCELADO = 0
                    };

                    var retorno = await cartaoRepository.RecargaCartaoCielo(recarga.NumCartao, recarga.Valor, ultimaMovimentacao, caixadepret, caixa, pagamentoCielo);
                    if (retorno == "")
                        return Ok("Cartão recarregado com sucesso.");
                    await cieloClient.Cancelar(autorizacao.Payment.PaymentId, autorizacao.Payment.Amount);
                    return BadRequest(retorno);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await cieloClient.Cancelar(autorizacao.Payment.PaymentId, autorizacao.Payment.Amount);
                }
            }
            return BadRequest(autorizacao);
        }

        [HttpGet("transacoes/{caixa}")]
        public async Task<List<PagamentoCielo>> ListarTransacoes([FromServices] CieloRepository cieloRepository, int caixa)
        {
            var transacoes = await cieloRepository.ObterTransacoes(caixa);
            return transacoes;
        }
    }
    
}