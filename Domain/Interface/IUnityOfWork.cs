using SiteSesc.Models.ProcessoSeletivo;

namespace SiteSesc.Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryDefault<psl_processoSeletivo> ProcSeletivoRepository { get; }
        // Outras propriedades para diferentes repositórios
        Task<int> CommitAsync();
    }
}
