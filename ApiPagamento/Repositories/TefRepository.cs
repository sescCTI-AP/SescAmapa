using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System;
using IBM.Data.DB2.Core;
using PagamentoApi.Models.Tef;
using PagamentoApi.Models;

namespace PagamentoApi.Repositories
{
    public class TefRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CobrancaRepository _cobrancaRepository;
        private readonly CaixaRepository _caixaRepository;

        public TefRepository(IConfiguration configuration, CobrancaRepository cobrancaRepository, CaixaRepository caixaRepository)
        {
            _configuration = configuration;
            _caixaRepository = caixaRepository;
            _cobrancaRepository = cobrancaRepository;
        }

        public async Task<dynamic> PagarCobranca(CobrancaTef cobranca)
        {
            try
            {
                using (var connection = new DB2Connection(_configuration.GetConnectionString("BDPROD")))
                {
                    connection.Open();
                    var caixa = await _caixaRepository.ObterCaixaAberto(true);
                    if (!(caixa is CACAIXA))
                    {
                        caixa = await _caixaRepository.AbrirCaixa(true);
                        if (!(caixa is CACAIXA))
                            return "Não existe um caixa aberto";
                    }
                    var cobrancaValor = await _cobrancaRepository.ObterCobranca(cobranca.IDCLASSE, cobranca.CDELEMENT, cobranca.SQCOBRANCA);
                    if (cobrancaValor.ValorRecebido != cobranca.Valor)
                    {
                        return "Valor incorreto";
                    }

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            var retorno = await _cobrancaRepository.PagamentoCobranca(caixa, cobrancaValor, connection, transaction);
                            //TODO Salvar dados da transação TEF

                            await transaction.CommitAsync();
                            if (retorno == "")
                            {
                                return "Cobrança paga com sucesso.";
                            }
                            else
                            {
                                return retorno;
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            await transaction.RollbackAsync();
                            return "Erro ao inserir pagamento";
                        }

                    }
                }
                return true;
            }

            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }

    }
}