using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IFreelancerRepository
    {
        public List<Job> ShowAllJobs(List<string> levels,string sortBy);

        public (Job job, Client client, Users user) JobDetails(int JobId);
        public List<Proposal> GetProposalsForFreelancer(int freelancerId);

        public int GetFreelancerIdByEmail(string email);

        public Proposal ProposalDetails(int ProposalId);
    }
}
