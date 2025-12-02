using AutoMapper;
using TravelRecommendations.Data.Dtos.Cities;
using TravelRecommendations.Data.Dtos.Countries;
using TravelRecommendations.Data.Dtos.Activities;
using TravelRecommendations.Data.Entities;

namespace TravelRecommendations.Data
{
    public class RestProfile : Profile
    {
        public RestProfile() 
        {
            CreateMap<Country, CountryDto>();
            CreateMap<CreateCountryDto, Country>();
            CreateMap<UpdateCountryDto, Country>();

            CreateMap<CreateCityDto, City>();
            CreateMap<UpdateCityDto, City>();
            CreateMap<City, CityDto>();

            CreateMap<CreateActivityDto, Activity>();
            CreateMap<UpdateActivityDto, Activity>();
            CreateMap<Activity, ActivityDto>();
        }
    }
}
