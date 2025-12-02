using System.ComponentModel.DataAnnotations;
using TravelRecommendations.Auth.Model;

namespace TravelRecommendations.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime CreationDateUtc { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Required]
        public required string UserId { get; set; }
        public User User { get; set; }
    }
}
