using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using System;

namespace SiteSesc.Repositories
{
    public class CidadeRepository
    {
        private readonly SiteSescContext _dbContext;

        public CidadeRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Cidade>> GetCidadesAtivas()
        {
            var cidade = await _dbContext.Cidade.Where(c => c.IsAtivo).ToListAsync();
            return cidade;
        }

        public async Task<Cidade> GetCidade(int? id)
        {
            return await _dbContext.Cidade.FindAsync(id);
        }

    }
}