namespace FreelanceMarketPlace.Models.Entities
{
    public class Users
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string CNIC { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PaypalEmail { get; set; }
        public bool Availability { get; set; }
        public string Phone { get; set; }
        public int AddressId { get; set; }
        public bool IsAdmin { get; set; }
        public string UserPassword { get; set; }
    }
}
