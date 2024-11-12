using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using System;

namespace SiteSesc.Repositories
{
    public class StatusRepository
    {
        private readonly SiteSescContext _dbContext;

        public StatusRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Status>> GetStatus()
        {
            var retorno = await _dbContext.Status.ToListAsync();
            return retorno;
        }
      
    }
}