using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelRecommendations.Auth.Model;
using TravelRecommendations.Data.Dtos.Activities;
using TravelRecommendations.Data.Dtos.Cities;
using TravelRecommendations.Data.Entities;
using TravelRecommendations.Data.Repositories;

namespace TravelRecommendations.Controllers
{
    [ApiController]
    [Route("api/countries/{countryId}/cities/{cityId}/activities")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivitiesRepository _activitiesRepository;
        private readonly IMapper _mapper;
        private readonly ICitiesRepository _citiesRepository;
        private readonly ICountriesRepository _countriesRepository;

        public ActivitiesController(IActivitiesRepository activitiesRepository, IMapper mapper, ICitiesRepository citiesRepository, ICountriesRepository countriesRepository)
        {
            _activitiesRepository = activitiesRepository;
            _mapper = mapper;
            _citiesRepository = citiesRepository;
            _countriesRepository = countriesRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ActivityDto>> GetAllAsync(int cityId)
        {
            var cities = await _activitiesRepository.GetAsync(cityId);
            return cities.Select(o => _mapper.Map<ActivityDto>(o));
        }

        [AllowAnonymous]
        [HttpGet("{activityId}")]
        public async Task<ActionResult<ActivityDto>> GetAsync(int cityId, int activityId)
        {
            var city = await _activitiesRepository.GetAsync(cityId, activityId);
            if (city == null) return NotFound();

            return Ok(_mapper.Map<ActivityDto>(city));
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<ActivityDto>> PostAsync(int countryId, int cityId, CreateActivityDto activityDto)
        {
            var country = await _countriesRepository.Get(countryId);
            if (country == null) return NotFound($"Couldn't find a country with id {countryId}");

            var city = await _citiesRepository.GetAsync(countryId, cityId);
            if (city == null) return NotFound($"Couldn't find a city with id of {cityId} in country with id {countryId}");

            if (string.IsNullOrWhiteSpace(activityDto.Name) || activityDto.Name.Length < 2)
                return UnprocessableEntity(new { message = "Activity name must be at least 2 characters long." });

            if (activityDto.Rating < 1 || activityDto.Rating > 10)
                return UnprocessableEntity(new { message = "Rating must be between 1 and 10." });

            var activity = _mapper.Map<Activity>(activityDto);
            activity.CityId = cityId;

            activity.UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            await _activitiesRepository.InsertAsync(activity);

            return Created($"/api/countries/{countryId}/cities/{cityId}/activities/{activity.Id}", _mapper.Map<ActivityDto>(activity));
        }

        [HttpPut("{activityId}")]
        [Authorize]
        public async Task<ActionResult<ActivityDto>> PutAsync(int countryId, int cityId, int activityId, UpdateActivityDto activityDto)
        {
            var country = await _countriesRepository.Get(countryId);
            if (country == null)
                return NotFound($"Couldn't find a country with id of {countryId}");

            var city = await _citiesRepository.GetAsync(countryId, cityId);
            if (city == null)
                return NotFound($"Couldn't find a city with id of {cityId} in country {countryId}");

            var oldActivity = await _activitiesRepository.GetAsync(cityId, activityId);
            if (oldActivity == null)
                return NotFound($"Couldn't find an activity with id of {activityId} in city {cityId}");

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin && userId != oldActivity.UserId)
            {
                return Forbid();
            }

            _mapper.Map(activityDto, oldActivity);

            await _activitiesRepository.UpdateAsync(oldActivity);

            return Ok(_mapper.Map<ActivityDto>(oldActivity));
        }

        [HttpDelete("{activityId}")]
        [Authorize]
        public async Task<ActionResult> DeleteAsync(int cityId, int activityId)
        {
            var activity = await _activitiesRepository.GetAsync(cityId, activityId);
            if (activity == null) return NotFound();

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin && userId != activity.UserId)
            {
                return Forbid();
            }

            await _activitiesRepository.DeleteAsync(activity);

            // 204
            return NoContent();
        }
    }
}
