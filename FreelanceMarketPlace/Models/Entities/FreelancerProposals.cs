namespace FreelanceMarketPlace.Models.Entities
{
    public class FreelancerProposals
    {
        public int FreelancerId { get; set; }
        public string Title { get; set; }
        public string Intro { get; set; }
        public decimal AmountReceived { get; set; }
        public string GithubLink { get; set; }
        public string LinkedinLink { get; set; }
        public int PerHourRate { get; set; }
        public int WorkingHours { get; set; }
        public int? CardId { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Proposal details
        public int ProposalId { get; set; }
        public string ProposalDescription { get; set; }
        public decimal ProposalBid { get; set; }
        public string CompletionTime { get; set; }

        public string ProposalStatus { get; set; }
        public int JobId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

}
