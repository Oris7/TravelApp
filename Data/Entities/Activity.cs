using System.ComponentModel.DataAnnotations;
using TravelRecommendations.Auth.Model;

namespace TravelRecommendations.Data.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        [Required]
        public required string UserId { get; set; }
        public User User { get; set; }
    }
}
