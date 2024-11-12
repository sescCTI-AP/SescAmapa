using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Models.Cardapio;
using SiteSesc.Models.ProcessoSeletivo;
using System;

namespace SiteSesc.Repositories
{
    public class DefaultRepository
    {
        private readonly SiteSescContext _dbContext;

        public DefaultRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Sexo>> GetSexo()
        {
            var lista = await _dbContext.Sexo.ToListAsync();
            return lista;
        }

        public async Task<List<EstadoCivil>> GetEstadoCivil()
        {
            var lista = await _dbContext.EstadoCivil.ToListAsync();
            return lista;
        }

        public async Task<List<Escolaridade>> GetEscolaridade()
        {
            var lista = await _dbContext.Escolaridade.ToListAsync();
            return lista;
        }

        public async Task<List<SituacaoProfissional>> GetSituacaoProfissional()
        {
            var lista = await _dbContext.SituacaoProfissional.ToListAsync();
            return lista;
        }

        public async Task<List<TipoDocumentoIdentificacao>> GetTipoDocumentoIdentificacao()
        {
            var lista = await _dbContext.TipoDocumentoIdentificacao.ToListAsync();
            return lista;
        }

        public async Task<DashboardUsuario> GetDashboard(int idUsuario)
        {
            var result = await _dbContext.DashboardUsuario.FirstOrDefaultAsync(a => a.IdUsuario == idUsuario && a.IsAtivo);
            return result;
        }

        public async Task<List<UnidadeOperacional>> ObtemUnidades()
        {
            try
            {
                var unidades = _dbContext.UnidadeOperacional.Include(u => u.Arquivo).ToList();
                return unidades;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<UnidadeOperacional>> ObtemUnidadesAtividades()
        {
            try
            {
                var idUndAtividades = await _dbContext.AtividadeOnLine.Where(a => a.IsAtivo).Select(a => a.IdUnidadeOperacional).Distinct().ToListAsync();
                var unidades = _dbContext.UnidadeOperacional.Include(u => u.Arquivo).Where(a => idUndAtividades.Contains(a.Id)).ToList();
                return unidades;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<UnidadeOperacional> GetUnidadePorCduop(int cduop)
        {
            try
            {
                var unidade = await _dbContext.UnidadeOperacional.SingleOrDefaultAsync(u => u.Cduop == cduop);
                if (unidade != null)
                {
                    return unidade;
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }

        public async Task<UnidadeOperacional> GetUop(int cduop)
        {
            try
            {
                return await _dbContext.UnidadeOperacional.FirstOrDefaultAsync(a => a.Cduop == cduop);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<UnidadeOperacional> GetUopById(int id)
        {
            try
            {
                return await _dbContext.UnidadeOperacional.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<cdp_ComposicaoCardapio>> ObtemCardapio()
        {
            try
            {
                var composicao = new List<cdp_ComposicaoCardapio>();
                int dia = 1;
                if (_dbContext.cdp_Cardapio.Any(c => c.IsAtivo == true))
                {
                    var culture = new System.Globalization.CultureInfo("pt-BR");
                    var hoje = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
                    var cardapio = _dbContext.cdp_Cardapio.First(c => c.IsAtivo == true);
                    var hora = DateTime.Now.Hour;
                    if (hoje.ToLower() == "segunda-feira")
                    {
                        dia = 1;
                    }
                    if (hoje.ToLower() == "terça-feira")
                    {
                        dia = 2;
                    }
                    if (hoje.ToLower() == "quarta-feira")
                    {
                        dia = 3;
                    }
                    if (hoje.ToLower() == "quinta-feira")
                    {
                        dia = 4;
                    }
                    if (hoje.ToLower() == "sexta-feira")
                    {
                        dia = 5;
                    }
                    if (_dbContext.cdp_ComposicaoCardapio.Any(c => c.DiaDaSemana == dia && c.IdCardapio == cardapio.Id))
                    {
                        composicao = _dbContext.cdp_ComposicaoCardapio.Include(c => c.ItemCardapio).Where(c => c.DiaDaSemana == dia && c.IdCardapio == cardapio.Id).ToList();
                    }
                }
                return composicao;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<UnidadeOperacional>> GetUnidadesCardapio()
        {
            var cardapiosAtivos = _dbContext.cdp_Cardapio.Where(a => a.DataInicio <= DateTime.Now.Date && a.DataFinal >= DateTime.Now.Date  && a.IsAtivo);
            if(cardapiosAtivos != null)
            {
                var idUnidades = cardapiosAtivos.Select(a => a.IdUnidadeOperacional);
                return await _dbContext.UnidadeOperacional.Where(a => idUnidades.Contains(a.Id)).ToListAsync();
            }
            return null;
        }

        public async Task<List<cdp_GrupoItemCardapio>> GetGrupoItemCardapio()
        {
            return _dbContext.cdp_GrupoItemCardapio.ToList();
        }

        
        public async Task<List<cdp_ComposicaoCardapio>> GetCardapioDoDia()
        {
            var composicao = new List<cdp_ComposicaoCardapio>();

            if (await _dbContext.cdp_Cardapio.AnyAsync(c => c.IsAtivo == true))
            {
                int dia = 1;
                var hoje = DateTime.Today.DayOfWeek.ToString();
                var hora = DateTime.Now.Hour;
                var cardapio = _dbContext.cdp_Cardapio.Where(c => c.IsAtivo == true);
                if (hoje.ToLower() == "monday")
                {
                    dia = 1;
                    if (hora > 14)
                    {
                        dia = 2;
                    }
                }
                if (hoje.ToLower() == "tuesday")
                {
                    dia = 2;
                    if (hora > 14)
                    {
                        dia = 3;
                    }
                }
                if (hoje.ToLower() == "wednesday")
                {
                    dia = 3;
                    if (hora > 14)
                    {
                        dia = 4;
                    }
                }
                if (hoje.ToLower() == "thursday")
                {
                    dia = 4;
                    if (hora > 14)
                    {
                        dia = 5;
                    }
                }
                if (hoje.ToLower() == "friday")
                {
                    dia = 5;
                    if (hora > 14)
                    {
                        dia = 1;
                    }
                }

                foreach (var card in cardapio)
                {
                    if (await _dbContext.cdp_ComposicaoCardapio.AnyAsync(c => c.DiaDaSemana == dia && c.IdCardapio == card.Id))
                    {
                        var itens = await _dbContext.cdp_ComposicaoCardapio.Include(c => c.ItemCardapio).Where(c => c.DiaDaSemana == dia && c.IdCardapio == card.Id).ToListAsync();

                        composicao.AddRange(itens);
                    }
                }



            }

            return composicao;


        }

    }
}