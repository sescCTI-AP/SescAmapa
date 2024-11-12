using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Services;
using System;

namespace SiteSesc.Repositories
{
    public class UnidadeRepository
    {
        private readonly SiteSescContext _dbContext;

        public UnidadeRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UnidadeOperacional>> GetUOAtiva()
        {
            var unidade = await _dbContext.UnidadeOperacional.Where(c => c.IsAtivo).ToListAsync();
            return unidade;
        }

        public async Task<UnidadeOperacional> GetUO(int? id)
        {
            return await _dbContext.UnidadeOperacional.FindAsync(id);
        }

        public async Task<UnidadeOperacional> GetUOName(string nome)
        {
            return await _dbContext.UnidadeOperacional.Include(a => a.Arquivo).FirstOrDefaultAsync(a => a.NameRoute == nome);
        }


    }
}