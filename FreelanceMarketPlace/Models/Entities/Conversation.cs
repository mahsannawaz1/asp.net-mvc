namespace FreelanceMarketPlace.Models.Entities
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public int ClientId { get; set; }
        public int FreelancerId { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
