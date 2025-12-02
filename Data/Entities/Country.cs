using System.ComponentModel.DataAnnotations;
using TravelRecommendations.Auth.Model;

namespace TravelRecommendations.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTimeUtc { get; set; }


        [Required]
        public required string UserId { get; set; }
        public User User { get; set; }
    }
}
