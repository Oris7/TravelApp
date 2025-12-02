using Microsoft.EntityFrameworkCore;
using TravelRecommendations.Data.Entities;

namespace TravelRecommendations.Data.Repositories
{
    public interface ICountriesRepository
    {
        Task<Country> Create(Country country);
        Task Delete(Country country);
        Task<Country> Get(int id);
        Task<IEnumerable<Country>> GetAll();
        Task<Country> Put(Country country);
    }

    public class CountriesRepository : ICountriesRepository
    {

        private readonly RestContext _restContext;

        public CountriesRepository(RestContext restContext)
        {
            _restContext = restContext;
        }

        public async Task<IEnumerable<Country>> GetAll()
        {
            return await _restContext.Countries.ToListAsync();
        }

        public async Task<Country> Get(int id)
        {
            return await _restContext.Countries.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Country> Create(Country country)
        {
            _restContext.Countries.Add(country);
            await _restContext.SaveChangesAsync();

            return country;
        }

        public async Task<Country> Put(Country country)
        {
            _restContext.Countries.Update(country);
            await _restContext.SaveChangesAsync();
            return country;
        }

        public async Task Delete(Country country)
        {
            _restContext.Countries.Remove(country);
            await _restContext.SaveChangesAsync();
        }
    }
}
