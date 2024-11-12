using System.Linq.Expressions;

namespace SiteSesc.Domain.Interface
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;
        void SaveChanges();
        bool Any<T>(Expression<Func<T, bool>> predicate) where T : class;


    }
}
