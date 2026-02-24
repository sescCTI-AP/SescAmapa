 using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IBM.Data.DB2.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PagamentoApi.Models;
using PagamentoApi.Models.Cielo;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Tef;
using PagamentoApi.Repositories;

namespace PagamentoApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class CobrancaController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private IWebHostEnvironment _env;
        public CobrancaController(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            _env = env;
        }

        [HttpGet]
        [Route("{cpf}/{ano}")]
        [Authorize]
        public async Task<List<COBRANCA>> ObterCobrancas([FromServices] ClientelaRepository clientelaRepository, [FromServices] CobrancaRepository cobrancaRepository, string cpf, int ano)
        {
            var usuario = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            if (usuario == null) return null;
            var cobrancas = cobrancaRepository.getCobrancas(usuario, ano);
            return cobrancas;
        }

        [HttpGet]
        [Route("proximas/{cpf}")]
        [Authorize]
        public async Task<List<CobrancaValor>> ObterProximasCobrancas([FromServices] ClientelaRepository clientelaRepository, [FromServices] CobrancaRepository cobrancaRepository, string cpf, int ano)
        {
            var cobrancas = await cobrancaRepository.obterProximasCobrancasEmAberto(cpf);
            return cobrancas;
        }

        [HttpGet("em-aberto/{cpf}/{ano}")]
        [Authorize]
        public async Task<List<CobrancaValor>> ObterCobrancasEmAberto([FromServices] ClientelaRepository clientelaRepository, [FromServices] CobrancaRepository cobrancaRepository, string cpf, int ano)
        {
            var usuario = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            if (usuario == null) return null;
            var cobrancas = await cobrancaRepository.obterCobrancasEmAberto(usuario, ano);

            return cobrancas.OrderBy(c => c.Vencimento).ToList();

        }


        [HttpGet("cliente/{cdelement}/{sqcobranca}")]
        [Authorize]
        public async Task<ClientelaSummary> ObterClientePorCobranca([FromServices] ClientelaRepository clientelaRepository, [FromServices] CobrancaRepository cobrancaRepository, string cdelement, int sqcobranca)
        {
            var cliente = await clientelaRepository.ObterClientePorCobranca(cdelement, sqcobranca);
            if (cliente == null) return null;

            return cliente;

        }


        [HttpPost("inadimplentes")]
        [Authorize]
        public async Task<List<ClientelaCobranca>> ObterClientesInadimplentes([FromServices] ClientelaRepository clientelaRepository, [FromServices] CobrancaRepository cobrancaRepository, [FromBody] ClienteInadimplente cliente)
        {
            //var usuario = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            //if (usuario == null) return null;
            var cobrancas = await cobrancaRepository.obterInadimplentes(cliente);

            return cobrancas;
        }

        [HttpGet("inadimplencia/{cpf}")]
        [Authorize]
        public async Task<bool> VerificaInadimplencia([FromServices] ClientelaRepository clientelaRepository, [FromServices] CobrancaRepository cobrancaRepository, string cpf)
        {
            var clienteTitular = new CLIENTELA();
            var cliente = await clientelaRepository.ObterClientePorCpfSemFoto(cpf);
            var hasCobrancaVencida = false;
            if (cliente != null)
            {
                if (cliente.CDUOTITUL != null)
                    clienteTitular = await clientelaRepository.ObterClientePorMatricula((int)cliente.CDUOTITUL, (int)cliente.SQTITULMAT);
                else
                    clienteTitular = cliente;
            }
            var grupoFamiliar = await clientelaRepository.ObterClientePorCpfSemCobrancas(clienteTitular.NUCPF);
            foreach (var cli in grupoFamiliar)
            {
                hasCobrancaVencida = await cobrancaRepository.VerificaInadimplencia(cli.CDUOP, cli.SQMATRIC);
                if (hasCobrancaVencida)
                {
                    return true;
                }
            }
            return false;

        }

        [HttpPost("valor-atualizado")]
        [Authorize]
        public async Task<ActionResult<dynamic>> ObterValorCobranca([FromServices] CobrancaRepository cobrancaRepository, [FromBody] COBRANCA cobranca)
        {
            var cobrancaBD = await cobrancaRepository.ObterCobranca(cobranca.IDCLASSE, cobranca.CDELEMENT, cobranca.SQCOBRANCA);
            return Ok(cobrancaBD);

        }

        [HttpPost]
        [Route("pagar")]
        [Authorize]
        public async Task<dynamic> PagarCobrancas([FromServices] CaixaRepository caixaRepository, [FromServices] CobrancaRepository cobrancaRepository, [FromBody] COBRANCA cobranca)
        {            
            var caixa = await caixaRepository.ObterCaixaAberto();
            if (!(caixa is CACAIXA))
            {
                return BadRequest("Não existe um caixa aberto");
            }
            var cobrancaValor = await cobrancaRepository.ObterCobranca(cobranca.IDCLASSE, cobranca.CDELEMENT, cobranca.SQCOBRANCA);
            //cobrancaValor = cobrancaRepository.GetValorCobrancaComJurosMulta(cobrancaValor);
            try
            {
                var retorno = await cobrancaRepository.PagamentoCobranca(caixa, cobrancaValor, null, null);

                if (retorno == "")
                {
                    return Ok("Cobrança paga com sucesso.");
                }
                else
                {
                    return BadRequest(retorno);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Erro ao inserir pagamento");
            }
        }


        [HttpPost]
        [Route("pagar-tef")]
        [Authorize]
        public async Task<dynamic> PagarCobrancaTEF([FromServices] CartaoRepository cartaoRepository, [FromServices] CaixaRepository caixaRepository,
                [FromServices] TotemRepository totemRepository,
                [FromServices] CobrancaRepository cobrancaRepository,
                [FromBody] TransacaoTotem transacao)
        {
            
            var transacaoJaIniciada = await totemRepository.TransacaoJaIniciada(transacao);

            if(transacaoJaIniciada)
            {
                return BadRequest("Transação já iniciada");
            }
            
            /*  
            #region SALVA A TRANSACAO EM UM ARQUIVO DE TEXTO

            var pastaDiaMesAno = DateTime.Now.ToString("ddMMyyyy");
            var diretorio = Path.Combine(_env.ContentRootPath, "..\\arquivos\\ApiTef\\" + pastaDiaMesAno);
            //var diretorio = Path.Combine(_env.ContentRootPath, "C:\\inetpub\\arquivos\\ApiBancoBrasil\\webhook\\" + pastaDiaMesAno);
            var nomeArquivo = $"{transacao.VALOR}_" + $"{transacao.CUPOM}" + $"_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.txt";           

            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            var destinoFisico = Path.Combine(diretorio, nomeArquivo);

            string json = JsonConvert.SerializeObject(transacao);

            using (StreamWriter writer = new StreamWriter(destinoFisico))
            {
                writer.Write(json);
            }
            #endregion

            */
    
            if (string.IsNullOrEmpty(transacao.CDELEMENT))
            {
                return BadRequest("Cliente não possui Cartão gerado");
            }

            var caixa = await caixaRepository.ObterCaixaAberto(true);
            if (!(caixa is CACAIXA))
            {
                caixa = await caixaRepository.AbrirCaixa(true);
                if (!(caixa is CACAIXA))
                    return "Não existe um caixa aberto";
            }

            var cdmoeda = transacao.TIPOPAGAMENTO == "1" ? 1413 : 1412;

            if (transacao.IDCLASSE == "CART")
            {
                try
                {
                    var numCartao = Convert.ToInt32(transacao.CDELEMENT);
                    var caixadepret = await caixaRepository.ObterCaixaDePret(caixa);
                    var ultimaMovimentacao = await cartaoRepository.ObterUltimaMovimentacaoCartaoCliente(numCartao, caixa);
                    //REALIZA RECARGA
                    var result = await cartaoRepository.RecargaCartao(numCartao, transacao.VALOR, ultimaMovimentacao, caixadepret, caixa, cdmoeda, true);
                    
                    if (result == "")
                    {
                        transacao.CAIXA = caixa.SQCAIXA;
                        transacao.SQDEPRET = caixadepret?.SQDEPRET;
                        var registraTransacao = await totemRepository.RegistraTransacao(transacao);
                        if (registraTransacao)
                        {
                            var cliente = await cartaoRepository.ObterClientePorNumCartao(transacao.CDELEMENT);
                            if (cliente != null)
                            {
                                return new TransacaoRecarga(cliente.NMCLIENTE.Trim(), cliente.NUCPF, cliente.CDUOP, cliente.SQMATRIC, cliente.NUDV, transacao.VALOR.ToString());
                            }
                        }
                        return null;
                    } 
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                var cobrancaValor = await cobrancaRepository.ObterCobranca(transacao.IDCLASSE, transacao.CDELEMENT, (int)transacao.SQCOBRANCA);

                var cobrancaEstaPaga = await cobrancaRepository.ObterCobrancaCompleta(transacao.IDCLASSE, transacao.CDELEMENT, (int)transacao.SQCOBRANCA);

                if (cobrancaEstaPaga.STRECEBIDO != 0)
                {
                    return BadRequest("Cobranca paga ou cancelada anteriormente.");
                }

                if (cobrancaValor.ValorRecebido != transacao.VALOR)
                {
                    return BadRequest("Valor incorreto");
                }
                //cobrancaValor = cobrancaRepository.GetValorCobrancaComJurosMulta(cobrancaValor);
                using (var connection = new DB2Connection(configuration.GetConnectionString("BDPROD")))
                {
                    await connection.OpenAsync();
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            //var cdmoeda = transacao.TIPOPAGAMENTO == "1" ? 3 : 2;
                            transacao.CAIXA = caixa.SQCAIXA;
                            var retorno = await cobrancaRepository.PagamentoCobranca(caixa, cobrancaValor, connection, transaction, cdmoeda);
                            await transaction.CommitAsync();
                        }
                        catch (Exception e)
                        {
                            await transaction.RollbackAsync();
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                var registraTransacao = await totemRepository.RegistraTransacao(transacao);
                if (registraTransacao)
                {
                    return cobrancaValor;
                }
            }

            return null;

        }



        //[HttpPost]
        //[Route("pagar-tef")]
        //[Authorize]
        //public async Task<dynamic> PagarCobrancaTEF([FromServices] CaixaRepository caixaRepository, 
        //    [FromServices] TotemRepository totemRepository, 
        //    [FromServices] CobrancaRepository cobrancaRepository, 
        //    [FromBody] CobrancaTef cobranca)
        //{
        //    var caixa = await caixaRepository.ObterCaixaAberto();

        //    if (!(caixa is CACAIXA))
        //    {
        //        return BadRequest("Não existe um caixa aberto");
        //    }
        //    var cobrancaValor = await cobrancaRepository.ObterCobranca(cobranca.IDCLASSE, cobranca.CDELEMENT, cobranca.SQCOBRANCA);
        //    if (cobrancaValor.ValorRecebido != cobranca.Valor)
        //    {
        //        return BadRequest("Valor incorreto");
        //    }
        //    //cobrancaValor = cobrancaRepository.GetValorCobrancaComJurosMulta(cobrancaValor);
        //    try
        //    {
        //        var retorno = await cobrancaRepository.PagamentoCobranca(caixa, cobrancaValor, null, null);
        //        var transacao = await totemRepository.RegistraTransacao(caixa, cobranca);

        //        if (retorno == "")
        //        {
        //            return Ok("Cobrança paga com sucesso.");
        //        }
        //        else
        //        {
        //            return BadRequest(retorno);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return BadRequest("Erro ao inserir pagamento");
        //    }

        //}






        // [HttpPost]
        // [Route("cancelar")]
        // [Authorize]
        // public async Task<dynamic> CancelarCobranca([FromServices] CobrancaRepository cobrancaRepository, [FromBody] COBRANCA cobranca)
        // {
        //     var cobrancaValor = await cobrancaRepository.ObterCobranca(cobranca.IDCLASSE, cobranca.CDELEMENT, cobranca.SQCOBRANCA);
        //     //cobrancaValor = cobrancaRepository.GetValorCobrancaComJurosMulta(cobrancaValor);
        //     var retorno = await cobrancaRepository.CancelaCobrancas(cobranca.IDCLASSE, cobranca.CDELEMENT);
        //     if (retorno == "")
        //     {
        //         return Ok("Cobrança cancelada com sucesso.");
        //     }
        //     else
        //     {
        //         return BadRequest(retorno);
        //     }
        // }

        [HttpPost]
        [Route("cancelar-turmas")]
        [Authorize]
        public async Task<dynamic> CancelarCobrancaTurmas([FromServices] AtividadesRepository atividadesRepository, [FromBody] List<string> turmas)
        {
            //turmas.Split('.');
            //var retorno = await cobrancaRepository.CancelaCobrancas(turmas);
            var retorno = await atividadesRepository.CancelaInscricaoTurma(turmas);

            if (retorno == "")
            {
                return Ok("Turmas cancelada com sucesso.");
            }
            else
            {
                return BadRequest(retorno);
            }
        }

        [HttpPost]
        [Route("cancelar-covid")]
        [Authorize]
        public async Task<dynamic> CancelarCobrancaCovid([FromServices] CobrancaRepository cobrancaRepository, [FromBody] List<string> turmas)
        {
            //turmas.Split('.');
            var retorno = await cobrancaRepository.CancelaCobrancas(turmas);

            if (retorno == "")
            {
                return Ok("Cobranças canceladas com sucesso.");
            }
            else
            {
                return BadRequest(retorno);
            }

        }

        /*
        [HttpPost]
        [Route("teste")]
        [Authorize]
        public async Task<dynamic> teste([FromServices] CobrancaRepository cobrancaRepository)
        {
            var isCobrancaEmAberto = await cobrancaRepository.IsCobrancaJaPaga("OCRID", "032807550328000403710023", 149);

            return BadRequest("Status da Cobrana = " + isCobrancaEmAberto);

        }*/
      
    }
}