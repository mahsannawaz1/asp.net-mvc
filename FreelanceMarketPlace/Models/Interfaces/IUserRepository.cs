using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IUserRepository
    {
        public (bool isAuthenticated, string role) Login(Users user);

        public bool SignUp(Users user,string role);

        public Users GetUserProfile(string email);

        public Client GetClientProfile(int userId);

        public Freelancer GetFreelancerProfile(int userId);

        public void EditFreelancerProfile(Users user, Freelancer freelancer);

        public void EditProfilePicture(string email, string profileUrl);
        public void EditClientProfile(Users user);

    }
}
