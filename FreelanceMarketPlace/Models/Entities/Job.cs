namespace FreelanceMarketPlace.Models.Entities
{
    public class Job
    {
        public int JobId { get; set; }
        public int ClientId { get; set; }
        public string JobDescription { get; set; }
        public decimal JobBudget { get; set; }
        public string JobStatus { get; set; }
        public string JobLevel { get; set; }
        public string CompletionTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CompletedOn { get; set; }

        public List<string> Skills { get; set; }
    }
}
