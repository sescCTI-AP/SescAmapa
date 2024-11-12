using SiteSesc.Models.ProcessoSeletivo;

namespace SiteSesc.Domain.Interface
{
    public interface IProcessoSeletivoRepository 
    {
        void RemoveAreas(int processoSeletivoId);
        void AddAreaToProcessoSeletivo(int processoSeletivoId, int areaId);
        void RemoveCidades(int processoSeletivoId);
        void AddCidadeToProcessoSeletivo(int processoSeletivoId, int cidadeId);
    }
}
