using System.Linq.Expressions;
using Data;
using Microsoft.EntityFrameworkCore;
using WebReader.Data.Interfaces;
using WebReader.Models;

namespace WebReader.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {

        protected WebReaderDataContext Database;
        protected DbSet<TEntity> DbSet;

        public Repository(WebReaderDataContext context)
        {
            Database = context;
            DbSet = Database.Set<TEntity>();
        }

        public virtual async Task<TEntity> Create(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            var entry = Database.Entry(entity);
            DbSet.Attach(entity);
            entry.State = EntityState.Modified;

            return entity;
        }

        public async Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate) => await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        public virtual async Task<TEntity> GetById(Guid id) => await DbSet.FindAsync(id);
        public virtual async Task<IEnumerable<TEntity>> GetAll() => await DbSet.ToListAsync();
        public virtual void DeleteById(Guid id) => DbSet.Remove(new TEntity { Id = id });
        public int SaveChanges() => Database.SaveChanges();

        public void Dispose()
        {
            Database.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}