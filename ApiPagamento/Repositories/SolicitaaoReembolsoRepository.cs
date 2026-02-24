using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Dapper;
using IBM.Data.DB2.Core;
using Microsoft.Extensions.Configuration;
using PagamentoApi.Models;
using PagamentoApi.Models.Partial;
using PagamentoApi.Models.Site;
using PagamentoApi.Models.Termo;
using SiteSesc.Models;

namespace PagamentoApi.Repositories
{
    public class SolicitaaoReembolsoRepository
    {
        private readonly IConfiguration configuration;
        public SolicitaaoReembolsoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<SolicitacaoReembolso>> GetSolicitacaoReembolso(string cpf)
        {

            using (var connection = new SqlConnection(configuration.GetConnectionString("SITE")))
            {
                await connection.OpenAsync();
                var sql = @"select Id, CpfCliente, CdElement, ValorReembolso, Justificativa, OpcaoRecebimento, NomeFavorecido, NomeBanco, " +
                          "       CpfFavorecido, Conta, TipoConta, Operacao, ChavePix, DataCadastro,IdUsuario, IdParentesco, " +
                          "       IdStatusReembolso, Agencia, Telefone, DataRevisao, IdUsuarioRevisao, MotivoIndeferido, ValorSolicitado " +
                          " from SolicitacaoReembolso where CpfCliente = @cpf";
                var solicitacao = await connection.QueryAsync<SolicitacaoReembolso>(
                        sql,
                        new
                        {
                            cpf = cpf
                        });
                
                return solicitacao.ToList();
            }
        }

        public async Task<TermoReembolsoAssinado> TermoReembolsoAssinado(string cpf, string cdelement)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("TERMOBD")))
            {
                await connection.OpenAsync();
                var sql = @"select Id, Cdelement, Cpf, Termo64, DataCadastro, NomeCliente, TipoSignature from TermoSignature where Cpf = @cpf and Cdelement = @cdelement and TipoSignature = 2";
                var termoAssinado = await connection.QueryFirstAsync<TermoReembolsoAssinado>(
                        sql,
                        new
                        {
                            cpf = cpf,
                            cdelement = cdelement
                        });

                return termoAssinado;
            }
        }

    }
}