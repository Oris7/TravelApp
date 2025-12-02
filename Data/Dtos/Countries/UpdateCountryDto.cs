using System.ComponentModel.DataAnnotations;

namespace TravelRecommendations.Data.Dtos.Countries
{
    public record UpdateCountryDto([Required]string Name);
}
