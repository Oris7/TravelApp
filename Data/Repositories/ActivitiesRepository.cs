using Microsoft.EntityFrameworkCore;
using TravelRecommendations.Data.Entities;

namespace TravelRecommendations.Data.Repositories
{
    public interface IActivitiesRepository
    {
        Task DeleteAsync(Activity activity);
        Task<List<Activity>> GetAsync(int cityId);
        Task<Activity> GetAsync(int cityId, int activityId);
        Task InsertAsync(Activity activity);
        Task UpdateAsync(Activity activity);
    }

    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly RestContext _restContext;
        public ActivitiesRepository(RestContext restContext)
        {
            _restContext = restContext;
        }

        public async Task<Activity> GetAsync(int cityId, int activityId)
        {
            return await _restContext.Activities.FirstOrDefaultAsync(o => o.CityId == cityId && o.Id == activityId);
        }

        public async Task<List<Activity>> GetAsync(int cityId)
        {
            return await _restContext.Activities.Where(o => o.CityId == cityId).ToListAsync();
        }

        public async Task InsertAsync(Activity activity)
        {
            _restContext.Activities.Add(activity);
            await _restContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Activity activity)
        {
            _restContext.Activities.Update(activity);
            await _restContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Activity activity)
        {
            _restContext.Activities.Remove(activity);
            await _restContext.SaveChangesAsync();
        }
    }
}
