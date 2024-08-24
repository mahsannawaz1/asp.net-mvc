namespace FreelanceMarketPlace.Models.Entities
{
    public class Client
    {
        public int ClientId { get; set; }
        public decimal AmountSpent { get; set; }
        public int JobCount { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
