using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelRecommendations.Auth.Model;
using TravelRecommendations.Data.Dtos.Cities;
using TravelRecommendations.Data.Dtos.Countries;
using TravelRecommendations.Data.Entities;
using TravelRecommendations.Data.Repositories;

namespace TravelRecommendations.Controllers
{
    [ApiController]
    [Route("api/countries/{countryId}/cities")]
    public class CitiesController : ControllerBase
    {


        private readonly ICitiesRepository _citiesRepository;
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;


        public CitiesController(ICitiesRepository citiesRepository, IMapper mapper, ICountriesRepository countriesRepository)
        {
            _citiesRepository = citiesRepository;
            _mapper = mapper;
            _countriesRepository = countriesRepository;
        }


        [HttpGet]
        public async Task<IEnumerable<CityDto>> GetAllAsync(int countryId)
        {
            var countries = await _citiesRepository.GetAsync(countryId);
            return countries.Select(o => _mapper.Map<CityDto>(o));
        }


        // /api/countries/1/cities/2
        [HttpGet("{cityId}")]
        public async Task<ActionResult<CityDto>> GetAsync(int countryId, int cityId)
        {
            var country = await _citiesRepository.GetAsync(countryId, cityId);
            if(country == null) return NotFound();

            return Ok(_mapper.Map<CityDto>(country));
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<CityDto>> PostAsync(int countryId, CreateCityDto cityDto)
        {
            var country = await _countriesRepository.Get(countryId);
            if (country == null) return NotFound($"Couldn't find a country with id of {countryId}");

            if (string.IsNullOrWhiteSpace(cityDto.Name) || cityDto.Name.Length < 2)
                return UnprocessableEntity(new { message = "City name must be at least 2 characters long." });

            var city = _mapper.Map<City>(cityDto);
            city.CountryId = countryId;

            city.UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            city.CreationDateUtc = DateTime.UtcNow;

            await _citiesRepository.InsertAsync(city);

            return Created($"/api/countries/{countryId}/cities/{city.Id}", _mapper.Map<CityDto>(city));
        }

        [HttpPut("{cityId}")]
        [Authorize]
        public async Task<ActionResult<CityDto>> PutAsync(int countryId, int cityId, UpdateCityDto cityDto)
        {
            var country = await _countriesRepository.Get(countryId);
            if (country == null) return NotFound($"Couldn't find a country with id of {countryId}");

            var oldCity = await _citiesRepository.GetAsync(countryId, cityId);
            if (oldCity == null) return NotFound();

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin && userId != oldCity.UserId)
            {
                return Forbid();
            }

            //oldCity.Name = cityDto.Name;
            _mapper.Map(cityDto, oldCity);

            await _citiesRepository.UpdateAsync(oldCity);

            return Ok(_mapper.Map<CityDto>(oldCity));
        }

        [HttpDelete("{cityId}")]
        [Authorize]
        public async Task<ActionResult> DeleteAsync(int countryId, int cityId)
        {
            var city = await _citiesRepository.GetAsync(countryId, cityId);
            if (city == null) return NotFound();

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin && userId != city.UserId)
            {
                return Forbid();
            }

            await _citiesRepository.DeleteAsync(city);

            // 204
            return NoContent();
        }
    }
}
