using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using System;

namespace SiteSesc.Repositories
{
    public class ParentescoRepository
    {
        private readonly SiteSescContext _dbContext;

        public ParentescoRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Parentesco>> GetParentescos()
        {
            var retorno = await _dbContext.Parentesco.ToListAsync();
            return retorno;
        }
      
    }
}