using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.Data.SqlClient;
namespace FreelanceMarketPlace.Models.Repositories
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public void SendProposal(Proposal proposal)
        {
            string query = @"
            INSERT INTO Proposal (ProposalDescription, ProposalBid, JobId, CompletionTime, FreelancerId)
            VALUES (@ProposalDescription, @ProposalBid, @JobId, @CompletionTime, @FreelancerId)";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FreelancerId", proposal.FreelancerId); 
                    cmd.Parameters.AddWithValue("@ProposalDescription", proposal.ProposalDescription); 
                    cmd.Parameters.AddWithValue("@ProposalBid", proposal.ProposalBid); 
                    cmd.Parameters.AddWithValue("@JobId", proposal.JobId); 
                    cmd.Parameters.AddWithValue("@CompletionTime", proposal.CompletionTime); 

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        // Show success message
                        Console.WriteLine("Proposal is sent successfully.");
                    }
                    catch (Exception ex)
                    {
                       
                        Console.WriteLine(ex.Message);
                       
                    }
                }
            }
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
        public List<Proposal> ShowAllProposalsOnJob(int jobId)
        {
            List<Proposal> proposals = new List<Proposal>();
            return proposals;
        }
        public List<Proposal> ShowAllSendProposals()
        {
            List<Proposal> proposals = new List<Proposal>(); 
            return proposals;
        }

        public int GetFreelancerIdByEmail(string email)
        {
            int userId = -1; // Assign a default value indicating no user found
            int clientId = -1; // Assign a default value indicating no client found

            using (SqlConnection connect = new SqlConnection(ConnectionString))
            {
                connect.Open();

                // Fetch UserId from Users table by email
                string userQuery = "SELECT UserId FROM Users WHERE UserEmail = @Email";
                using (SqlCommand cmd = new SqlCommand(userQuery, connect))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            userId = Convert.ToInt32(reader["UserId"]);
                        }
                    }
                }

                // If UserId is found, fetch the FreelancerId from the Freelancer table using the UserId as a foreign key
                if (userId != -1)
                {
                    string clientQuery = "SELECT FreelancerId FROM Freelancer WHERE UserId = @UserId";
                    using (SqlCommand cmd = new SqlCommand(clientQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                clientId = Convert.ToInt32(reader["FreelancerId"]);
                            }
                        }
                    }
                }
            }

            return clientId; // Return the found ClientId or -1 if no client was found
        }
    }
}
