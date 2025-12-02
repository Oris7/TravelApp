using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace TravelRecommendations.Auth.Model
{
    public class Roles
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public const string Guest = nameof(Guest);

        public static readonly IReadOnlyCollection<string> All = new[] { Admin, User, Guest };
    }
}
