using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IProposalRepository
    {
        public void SendProposal(Proposal proposal);
        public Proposal GetProposal(int proposalId);

        public void UpdateProposalStatus(int proposalId,string status);
        public void DeleteProposal(int proposalId);
        public void UpdateProposal(Proposal proposal);
        public List<Proposal> ShowAllProposalsOnJob(int jobId);
        public List<FreelancerProposals> ShowAllSendProposals(int freelancerId);

        public int GetFreelancerIdByEmail(string email);
    }
}
