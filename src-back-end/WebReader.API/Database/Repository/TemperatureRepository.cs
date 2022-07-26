using WebReader.API.Database.Context;
using WebReader.API.Database.Interfaces;
using WebReader.API.Domain.Models;

namespace WebReader.API.Database.Repository
{
    public class TemperatureRepository : Repository<Temperature>, ITemperatureRepository
    {
        public TemperatureRepository(ApplicationContext context) : base(context) { }

    }
}
