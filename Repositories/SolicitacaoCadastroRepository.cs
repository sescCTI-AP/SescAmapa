using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SiteSesc.Data;
using SiteSesc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SiteSesc.Repositories
{
    public class SolicitacaoCadastroRepository
    {
        private readonly SiteSescContext _dbContext;

        public SolicitacaoCadastroRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SolicitacaoCadastroCliente>> GetAllSoliticacoes(int? tipo = null)
        {
            if (tipo != null)
            {
                return await _dbContext.SolicitacaoCadastroCliente.Include(a => a.Usuario).Include(a => a.Status).Where(a => a.IdStatus == tipo).ToListAsync();

            }
            return await _dbContext.SolicitacaoCadastroCliente.Include(a => a.Usuario).Include(a => a.Status).ToListAsync();
        }

        public int GetTotalSoliticacoes()
        {
            return _dbContext.SolicitacaoCadastroCliente.Count();
        }

        public int GetTotalFinalizadas()
        {
            return _dbContext.SolicitacaoCadastroCliente.Count(a => a.IdStatus == 8);
        }

        public int GetTotaPleno()
        {
            return _dbContext.SolicitacaoCadastroCliente.Count(a => a.CnpjEmpresa != null);
        }

        public int GetTotaAtividades()
        {
            return _dbContext.SolicitacaoCadastroCliente.Count(a => a.CnpjEmpresa == null);
        }

        public int GetTotalCorrecoes()
        {
            return _dbContext.HstSolicitacao.Count(a => a.Observacao == "Cliente reenviou formulário para análise");
        }

        public int GetNovasSolicitacoes()
        {
            return _dbContext.HstSolicitacao.Count(a => a.Observacao == "Cliente enviou nova solicitação de cadastro.");
        }

        public bool HasSolicitacaoAnaliseCliente(string cpf)
        {
            return _dbContext.SolicitacaoCadastroCliente.Any(s => s.Usuario.Cpf == cpf && s.IdStatus <= 4);
        }

        public async Task<HstSolicitacao> GetSoliticacaoCorrecao(string cpf)
        {
            var solicitacao = await _dbContext.SolicitacaoCadastroCliente.Include(s => s.Usuario).FirstOrDefaultAsync(s => s.Usuario.Cpf == cpf && s.IdStatus <= 4);
            if (solicitacao != null)
            {
                var historico = await _dbContext.HstSolicitacao.Where(a => a.IdSolicitacaoCadastroCliente == solicitacao.Id).ToListAsync();
                if (historico.Any())
                {
                    var hst = historico.OrderByDescending(a => a.DataCadastro).FirstOrDefault();
                    if (!hst.IsCliente)
                    {
                        return hst;
                    }
                }
            }
            return null;
        }

        public async Task<bool> VerificarExisteSolicitacao(string cpf, string categoria)
        {
            try
            {
                var status = 1;
                if (categoria  == "renovacao") status = 9;
                var solicitacao = await _dbContext.SolicitacaoCadastroCliente
                    .FirstOrDefaultAsync(s => s.Cpf == cpf && s.IdStatus <= status);

                if(solicitacao == null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

        }

        public async Task<SolicitacaoCadastroCliente> GetSoliticacao(Guid? guid)
        {
            var solicitacao = await _dbContext.SolicitacaoCadastroCliente
                .Include(a => a.Usuario)
                .Include(a => a.Status)
                .Include(a => a.EstadoCivil)
                .Include(a => a.Escolaridade)
                .Include(a => a.Parentesco)
                .Include(a => a.Sexo)
                .Include(a => a.SituacaoProfissional)
                .FirstOrDefaultAsync(a => a.Id == guid);
            return solicitacao;
        }

        public async Task<List<ArquivoCadastroCliente>> GetArquivosSolicitacao(Guid? guid)
        {
            var listaArquivosSolicitacao = await _dbContext.ArquivoCadastroCliente.Include(a => a.Arquivo).Where(a => a.IdSolicitacaoCadastroCliente == guid).ToListAsync();
            return listaArquivosSolicitacao;
        }

        //public async Task<SolicitacaoCadastroCliente> Save(SolicitacaoCadastroCliente solicitacao)
        //{
        //    if (solicitacao != null)
        //    {
        //        using (var transaction = _dbContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                try
        //                {
        //                    await _dbContext.SolicitacaoCadastroCliente.AddAsync(solicitacao);                           
        //                    await _dbContext.SaveChangesAsync();

        //                    var hst = new HstSolicitacao
        //                    {
        //                        Observacao = "Cliente enviou nova solicitação de cadastro.",
        //                        IsCliente = true,
        //                        IdUsuario = solicitacao.IdUsuario,
        //                        IdSolicitacaoCadastroCliente = solicitacao.Id
        //                    };

        //                    var addHst = await AddHistorico(hst, 1);

        //                    await transaction.CommitAsync();
        //                    return solicitacao;
        //                }
        //                catch (Exception ex)
        //                {
        //                    await transaction.RollbackAsync();
        //                    Console.WriteLine(ex.Message);
        //                    return null;
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.Message);
        //                return null;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public async Task<bool> AddHistorico(HstSolicitacao historico, int? IdStatus)
        //{
        //    using (var transaction = _dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            await _dbContext.HstSolicitacao.AddAsync(historico);
        //            await _dbContext.SaveChangesAsync();

        //            if (IdStatus != null)
        //            {
        //                var solicitacao = await GetSoliticacao(historico.IdSolicitacaoCadastroCliente);
        //                if (solicitacao != null)
        //                {
        //                    solicitacao.IdStatus = (int)IdStatus;
        //                    _dbContext.Entry(solicitacao).State = EntityState.Modified;
        //                    await _dbContext.SaveChangesAsync();
        //                }
        //            }
        //            await transaction.CommitAsync();
        //            return true;
        //        }
        //        catch (Exception e)
        //        {
        //            await transaction.RollbackAsync();
        //            Console.WriteLine(e.Message);
        //            return false;
        //        }
        //    }
        //}


        public async Task<SolicitacaoCadastroCliente> Save(SolicitacaoCadastroCliente solicitacao)
        {
            if (solicitacao != null)
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var status = 1;
                        if (solicitacao.TipoCategoria.ToLower() == "renovacao")
                            status = 9;

                        if (solicitacao.IdSexo == 0)
                            solicitacao.IdSexo = 1;
                        else if (solicitacao.IdSexo == 1)
                            solicitacao.IdSexo = 2;

                        await _dbContext.SolicitacaoCadastroCliente.AddAsync(solicitacao);
                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var observacao = "Cliente enviou nova solicitação de cadastro.";
                        if (status == 9)
                            observacao = "Cliente solicitou renovação da credencial";

                        var hst = new HstSolicitacao
                        {
                            Observacao = observacao,
                            IsCliente = true,
                            IdUsuario = solicitacao.IdUsuario,
                            IdSolicitacaoCadastroCliente = solicitacao.Id
                        };

                        var addHst = await AddHistorico(hst, status);

                        if (!addHst)
                        {
                            throw new Exception("Erro ao adicionar histórico");
                        }


                        return solicitacao;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine(ex.Message);
                        return null;
                    }
                }
            }
            return null;
        }

        public async Task<bool> AddHistorico(HstSolicitacao historico, int? IdStatus)
        {
            try
            {
                await _dbContext.HstSolicitacao.AddAsync(historico);

                if (IdStatus != null)
                {
                    var solicitacao = await GetSoliticacao(historico.IdSolicitacaoCadastroCliente);
                    if (solicitacao != null)
                    {
                        solicitacao.IdStatus = (int)IdStatus;
                        _dbContext.Entry(solicitacao).State = EntityState.Modified;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }




        public async Task<List<HstSolicitacao>> GetHistorico(Guid? id)
        {
            if (id == null)
                return null;

            return await _dbContext.HstSolicitacao.Include(a => a.Usuario).Where(a => a.IdSolicitacaoCadastroCliente == id).ToListAsync();
        }

    }
}