using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IFreelancerRepository
    {
        public List<Job> ShowAllJobs();

        public (Job job, Client client, Users user) JobDetails(int JobId);
    }
}
