using Microsoft.EntityFrameworkCore;
using SiteSesc.Data;
using SiteSesc.Domain.Interface;
using SiteSesc.Models.ProcessoSeletivo;
using SiteSesc.Models.Edital;

namespace SiteSesc.Domain.Abstract
{
    public class Repository<T> : IRepositoryDefault<T> where T : class
    {
        protected readonly SiteSescContext _context;

        public Repository(SiteSescContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null) _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveAreas(int processoSeletivoId)
        {
            var areas = _context.psl_areasProcessoSeletivo.Where(a => a.IdProcessoSeletivo == processoSeletivoId);
            _context.psl_areasProcessoSeletivo.RemoveRange(areas);
        }

        public void RemoveAreasEdital(int editalId)
        {
            var areas = _context.edt_areasEdital.Where(a => a.IdEdital == editalId);
            _context.edt_areasEdital.RemoveRange(areas);
        }

        public void AddAreaToProcessoSeletivo(int processoSeletivoId, int areaId)
        {
            var novaArea = new psl_areasProcessoSeletivo { IdProcessoSeletivo = processoSeletivoId, IdArea = areaId };
            _context.psl_areasProcessoSeletivo.Add(novaArea);
        }

        public void AddAreaToEdital(int editalId, int areaId)
        {
            var novaArea = new edt_areasEdital { IdEdital = editalId, IdArea = areaId };
            _context.edt_areasEdital.Add(novaArea);
        }

        public void RemoveCidades(int processoSeletivoId)
        {
            var cidades = _context.psl_cidadesProcessoSeletivo.Where(c => c.IdProcessoSeletivo == processoSeletivoId);
            _context.psl_cidadesProcessoSeletivo.RemoveRange(cidades);
        }

        public void RemoveCidadesEdital(int editalId)
        {
            var cidades = _context.edt_cidadesEdital.Where(c => c.IdEdital == editalId);
            _context.edt_cidadesEdital.RemoveRange(cidades);
        }

            public void AddCidadeToProcessoSeletivo(int processoSeletivoId, int cidadeId)
        {
            var novaCidade = new psl_cidadesProcessoSeletivo { IdProcessoSeletivo = processoSeletivoId, IdCidade = cidadeId };
            _context.psl_cidadesProcessoSeletivo.Add(novaCidade);
        }

        public void AddCidadeToEdital(int editalId, int cidadeId)
        {
            var novaCidade = new edt_cidadesEdital{ IdEdital = editalId, IdCidade = cidadeId };
            _context.edt_cidadesEdital.Add(novaCidade);
        }
    }
}
