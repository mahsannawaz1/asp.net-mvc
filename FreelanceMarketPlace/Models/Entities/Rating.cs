namespace FreelanceMarketPlace.Models.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set; }
        public string Feedback { get; set; }
        public int ClientId { get; set; }
        public int FreelancerId { get; set; }
        public int JobId { get; set; }
    }
}
