namespace FreelanceMarketPlace.Models.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Sender { get; set; }
        public string MessageContent { get; set; }
        public int ConversationId { get; set; }
        public DateTime SendAt { get; set; }
    }
}
