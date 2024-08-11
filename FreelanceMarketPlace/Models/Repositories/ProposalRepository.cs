using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class ProposalRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public void SendProposal(Proposal proposal)
        {

        }
        public Proposal GetProposal(int proposalId)
        {
            return new Proposal();
        }
        public void DeleteProposal(int proposalId)
        {

        }
        public void UpdateProposal(Proposal proposal)
        {

        }
        public List<Proposal> ShowAllProposalSOnJob(int jobId)
        {
            List<Proposal> proposals = new List<Proposal>();
            return proposals;
        }
        public List<Proposal> ShowAllSendProposals()
        {
            List<Proposal> proposals = new List<Proposal>(); 
            return proposals;
        }
    }
}
