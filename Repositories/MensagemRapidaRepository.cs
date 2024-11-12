using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using System;

namespace SiteSesc.Repositories
{
    public class MensagemRapidaRepository
    {
        private readonly SiteSescContext _dbContext;

        public MensagemRapidaRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MensagemRapida>> GetMensagensRapidas()
        {
            var msg = await _dbContext.MensagemRapida.ToListAsync();
            return msg;
        }
      
    }
}