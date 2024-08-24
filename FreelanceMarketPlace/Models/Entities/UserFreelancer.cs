namespace FreelanceMarketPlace.Models.Entities
{
    public class UserFreelancer
    {
        public int UserId { get; set; } 
        public string UserEmail { get; set; }
        public string? CNIC { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PaypalEmail { get; set; }
        public int Availability { get; set; }
        public string? Phone { get; set; }
        public string Title { get; set; }
        public string Intro { get; set; }
        public string GithubLink { get; set; }
        public string LinkedInLink { get; set; }
        public decimal PerHourRate { get; set; }
    }
}
