using System.ComponentModel.DataAnnotations;

namespace TravelRecommendations.Data.Dtos.Cities
{
    public record CreateCityDto([Required]string Name);
}
