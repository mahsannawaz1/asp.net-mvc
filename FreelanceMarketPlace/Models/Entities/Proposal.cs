namespace FreelanceMarketPlace.Models.Entities
{
    public class Proposal
    {
        public int ProposalId { get; set; }
        public int FreelancerId { get; set; }
        public string ProposalDescription { get; set; }
        public decimal ProposalBid { get; set; }
        public string ProposalStatus { get; set; }
        public int JobId { get; set; }
        public string CompletionTime { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
