using TravelRecommendations.Data.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TravelRecommendations.Data.Repositories
{
    public interface ICitiesRepository
    {
        Task DeleteAsync(City city);
        Task<List<City>> GetAsync(int countryId);
        Task<City> GetAsync(int countryId, int cityId);
        Task InsertAsync(City city);
        Task UpdateAsync(City city);
    }

    public class CitiesRepository : ICitiesRepository
    {

        private readonly RestContext _restContext;
        public CitiesRepository(RestContext restContext)
        {
            _restContext = restContext;
        }

        public async Task<City> GetAsync(int countryId, int cityId)
        {
            return await _restContext.Cities.FirstOrDefaultAsync(o => o.CountryId == countryId && o.Id == cityId);
        }

        public async Task<List<City>> GetAsync(int countryId)
        {
            return await _restContext.Cities.Where(o => o.CountryId == countryId).ToListAsync();
        }

        public async Task InsertAsync(City city)
        {
            _restContext.Cities.Add(city);
            await _restContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(City city)
        {
            _restContext.Cities.Update(city);
            await _restContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(City city)
        {
            _restContext.Cities.Remove(city);
            await _restContext.SaveChangesAsync();
        }
    }
}
