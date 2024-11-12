using SiteSesc.Data;
using SiteSesc.Domain.Abstract;
using SiteSesc.Domain.Interface;
using SiteSesc.Models.ProcessoSeletivo;

namespace SiteSesc.Repositories
{
    public class ProcSeletivoRepository : Repository<psl_processoSeletivo>
    {
        private readonly SiteSescContext _context;

        public ProcSeletivoRepository(SiteSescContext context) : base(context)
        {
            _context = context;
        }

        public void RemoveAreas(int processoSeletivoId)
        {
            var areas = _context.psl_areasProcessoSeletivo.Where(a => a.IdProcessoSeletivo == processoSeletivoId);
            _context.psl_areasProcessoSeletivo.RemoveRange(areas);
        }

        public void AddAreaToProcessoSeletivo(int processoSeletivoId, int areaId)
        {
            var novaArea = new psl_areasProcessoSeletivo { IdProcessoSeletivo = processoSeletivoId, IdArea = areaId };
            _context.psl_areasProcessoSeletivo.Add(novaArea);
        }

        public void RemoveCidades(int processoSeletivoId)
        {
            var cidades = _context.psl_cidadesProcessoSeletivo.Where(c => c.IdProcessoSeletivo == processoSeletivoId);
            _context.psl_cidadesProcessoSeletivo.RemoveRange(cidades);
        }

        public void AddCidadeToProcessoSeletivo(int processoSeletivoId, int cidadeId)
        {
            var novaCidade = new psl_cidadesProcessoSeletivo { IdProcessoSeletivo = processoSeletivoId, IdCidade = cidadeId };
            _context.psl_cidadesProcessoSeletivo.Add(novaCidade);
        }
    }
}
