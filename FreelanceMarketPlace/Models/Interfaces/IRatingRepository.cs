using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IRatingRepository
    {
        void AddRating(Rating rating);
        Rating GetRating(int ratingId);
        void UpdateRating(Rating rating);
        void DeleteRating(int ratingId);
        List<Rating> GetRatingsForUser(int userId); // Optional: Retrieve ratings for a specific user
    }
}
