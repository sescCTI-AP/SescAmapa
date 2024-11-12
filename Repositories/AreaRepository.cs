using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Models;
using SiteSesc.Services;
using System;

namespace SiteSesc.Repositories
{
    public class AreaRepository
    {
        private readonly SiteSescContext _dbContext;

        public AreaRepository(SiteSescContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Area>> GetAreasAtivas()
        {
            var area = await _dbContext.Area.Include(a => a.Arquivo).Where(c => c.IsAtivo).ToListAsync();
            return area;
        }

        public async Task<Area> GetArea(int? id)
        {
            return await _dbContext.Area.FindAsync(id);
        }

        public async Task<Area> GetAreaPorNome(string nome)
        {
            return await _dbContext.Area.Include(a => a.Arquivo).FirstOrDefaultAsync(a => a.NameRoute == nome);
        }

        public async Task<List<SubArea>> GetSubArea()
        {
            var subarea = _dbContext.SubArea.Include(s => s.Arquivo).Include(s => s.Area).OrderBy(c => c.Area).ThenBy(s => s.Nome).ToList();
            return subarea;
        }

        public async Task<List<SubArea>> GetSubAreaPorArea(int idArea)
        {
            var subarea = _dbContext.SubArea.Where(a => a.IdArea == idArea).ToList();
            return subarea;
        }

    }
}