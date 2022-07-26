using WebReader.API.Database.Context;
using WebReader.API.Domain.Models;

namespace WebReader.API.Database.Repository
{
    public class UserRepository<TEntity> : IUserRepository<TEntity>
    {
        public UserRepository(ApplicationContext context) : base(context) { }
    }
}
