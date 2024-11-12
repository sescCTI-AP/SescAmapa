namespace SiteSesc.Domain.Interface
{
    public interface IRepositoryDefault<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        void RemoveAreas(int processoSeletivoId);

        void RemoveAreasEdital(int editalId);
        void AddAreaToProcessoSeletivo(int processoSeletivoId, int areaId);
        void AddAreaToEdital(int processoSeletivoId, int areaId);

        void RemoveCidades(int processoSeletivoId);
        void RemoveCidadesEdital(int editalId);
        void AddCidadeToProcessoSeletivo(int processoSeletivoId, int cidadeId);
        void AddCidadeToEdital(int processoSeletivoId, int cidadeId);
    }
}
