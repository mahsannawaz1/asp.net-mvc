namespace FreelanceMarketPlace.Models.Entities
{
    public class UserClient 
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string? CNIC { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PaypalEmail { get; set; }
        public int Availability { get; set; }
        public string? Phone { get; set; }
    }
}
