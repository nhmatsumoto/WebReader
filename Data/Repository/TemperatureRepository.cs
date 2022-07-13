using Data;
using WebReader.Data.Interfaces;
using WebReader.Models;

namespace WebReader.Data.Repository
{
    public class TemperatureRepository : Repository<Temperature>, ITemperatureRepository
    {
        public TemperatureRepository(WebReaderDataContext context) : base(context)
        {

        }
    }
}