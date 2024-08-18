using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IProposalRepository
    {
        public void SendProposal(Proposal proposal);
        public Proposal GetProposal(int proposalId);
        public void DeleteProposal(int proposalId);
        public void UpdateProposal(Proposal proposal);
        public List<Proposal> ShowAllProposalsOnJob(int jobId);
        public List<Proposal> ShowAllSendProposals();

        public int GetFreelancerIdByEmail(string email);
    }
}
