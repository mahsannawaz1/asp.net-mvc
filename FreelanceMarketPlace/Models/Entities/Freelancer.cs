namespace FreelanceMarketPlace.Models.Entities
{
    public class Freelancer
    {
        public int FreelancerId { get; set; }
        public string Title { get; set; }
        public string Intro { get; set; }
        public decimal AmountReceived { get; set; }
        public string GithubLink { get; set; }
        public string LinkedInLink { get; set; }
        public decimal PerHourRate { get; set; }
        public string WorkingHours { get; set; }

        public int ProposalCount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
