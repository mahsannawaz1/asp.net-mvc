using FreelanceMarketPlace.Models.Entities;


namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IJobRepository
    {
        public void CreateJob(Job job);
        public (Job job, Client client, Users user, List<FreelancerProposals> proposalWithFreelancer) GetJob(int jobId);
        
        public void UpdateJob(Job job);
        public void DeleteJob(int jobId);
        public List<Job> ShowAllJobs(int ClientId);
        public int GetClientIdByEmail(string email);

        public FreelancerProposals GetProposalByIdOnJob(int proposalId);
    }
}
