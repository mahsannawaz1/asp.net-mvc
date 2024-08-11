using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IUserRepository
    {
        public bool Login(Users user);

        public bool SignUp(Users user,string role);

        public Users Profile(Users user);

    }
}
