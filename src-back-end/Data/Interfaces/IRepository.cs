using System.Linq.Expressions;

namespace WebReader.Data.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> GetById(Guid id);
        TEntity Update(TEntity entity);
        Task<IEnumerable<TEntity>> GetAll();
        void DeleteById(Guid id);
        Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate);
        int SaveChanges();

    }
}