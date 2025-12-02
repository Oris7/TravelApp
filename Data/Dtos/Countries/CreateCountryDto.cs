using System.ComponentModel.DataAnnotations;

namespace TravelRecommendations.Data.Dtos.Countries
{
    public record CreateCountryDto([Required] string Name);
}
