using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelRecommendations.Auth.Model;
using TravelRecommendations.Data.Dtos.Countries;
using TravelRecommendations.Data.Entities;
using TravelRecommendations.Data.Repositories;

namespace TravelRecommendations.Controllers
{
    /*
    countries
    /api/countries GET ALL 200
    /api/countries/{id} GET 200
    /api/countries POST 201
    /api/countries/{id} PUT 200
    /api/countries/{id} DELETE 200/204
    */


    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesRepository _countriesRepository;
        private readonly IMapper _mapper;
        public CountriesController(ICountriesRepository countriesRepository, IMapper mapper)
        {
            _countriesRepository = countriesRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<CountryDto>> GetAll()
        {
            return (await _countriesRepository.GetAll()).Select(o => _mapper.Map<CountryDto>(o));
        }

        [HttpGet(template:"{id}")]
        public async Task<ActionResult<CountryDto>> Get(int id)
        {
            var country = await _countriesRepository.Get(id);
            if(country == null) return NotFound($"Country with id '{id}' not found.");

            return Ok(_mapper.Map<CountryDto>(country));
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<CountryDto>> Post(CreateCountryDto countryDto)
        {
            if (string.IsNullOrWhiteSpace(countryDto.Name) || countryDto.Name.Length < 2)
                return UnprocessableEntity(new { message = "Country name must be at least 2 characters long." });

            var country = _mapper.Map<Country>(countryDto);

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            country.UserId = userId;

            await _countriesRepository.Create(country);

            // 201
            // Created country
            return Created($"/api/countries/{country.Id}", _mapper.Map<CountryDto>(country));
        }


        [HttpPut(template: "{id}")]
        [Authorize]
        public async Task<ActionResult<CountryDto>> Put(int id, UpdateCountryDto countryDto)
        {
            var country = await _countriesRepository.Get(id);
            if (country == null) return NotFound($"Country with id '{id}' not found.");

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin && userId != country.UserId)
            {
                return Forbid();
            }

            //country.Name = countryDto.Name; arba
            _mapper.Map(countryDto, country); // bet taip reik su visais atributais

            await _countriesRepository.Put(country);

            return Ok(_mapper.Map<CountryDto>(country));
        }

        [HttpDelete(template: "{id}")]
        [Authorize]
        public async Task<ActionResult<CountryDto>> Delete(int id)
        {
            var country = await _countriesRepository.Get(id);
            if (country == null) return NotFound($"Country with id '{id}' not found.");

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var isAdmin = User.IsInRole(Roles.Admin);

            if (!isAdmin && userId != country.UserId)
            {
                return Forbid();
            }

            await _countriesRepository.Delete(country);

            // 204
            return NoContent();
        }
    }
}
