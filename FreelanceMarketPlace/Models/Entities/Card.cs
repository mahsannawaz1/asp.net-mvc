namespace FreelanceMarketPlace.Models.Entities
{
    public class Card
    {
        public int CardId { get; set; }
        public int CardCvv { get; set; }
        public DateTime CardExpiry { get; set; }
        public string CardNumber { get; set; }
    }
}
