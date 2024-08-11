namespace FreelanceMarketPlace.Models.Entities
{
    public class Contract
    {
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public int FreelancerId { get; set; }
        public int ProposalId { get; set; }
        public string ContractDescription { get; set; }
        public string CompletionTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
