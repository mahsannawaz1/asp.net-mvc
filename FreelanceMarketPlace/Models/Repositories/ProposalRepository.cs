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
            return new List<Proposal>();
        }

        public List<FreelancerProposals> ShowAllSendProposals(int freelancerId)
        {
            List<FreelancerProposals> freelancerProposals = new List<FreelancerProposals>();

            string query = @"
        SELECT 
            F.FreelancerId,
            F.Title,
            F.Intro,
            F.AmountReceived,
            F.GithubLink,
            F.LinkedinLink,
            F.PerHourRate,
            F.WorkingHours,
            F.CardId,
            F.UserId,
            U.UserEmail,
            U.FirstName,
            U.LastName,
            P.ProposalId,
            P.ProposalDescription,
            P.ProposalBid,
            P.CompletionTime,
            P.JobId,
            P.CreatedOn,
            P.UpdatedOn
        FROM 
            Freelancer F
        JOIN 
            Users U ON F.UserId = U.UserId
        JOIN 
            Proposal P ON P.FreelancerId = F.FreelancerId
        WHERE 
            F.FreelancerId = @FreelancerId";  // Fixed WHERE clause

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FreelancerId", freelancerId);

                        try
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    FreelancerProposals freelancerProposal = new FreelancerProposals
                                    {
                                        FreelancerId = Convert.ToInt32(reader["FreelancerId"]),
                                        Title = reader["Title"].ToString(),
                                        Intro = reader["Intro"].ToString(),
                                        AmountReceived = reader["AmountReceived"] != DBNull.Value ? (decimal)reader["AmountReceived"] : 0,
                                        GithubLink = reader["GithubLink"].ToString(),
                                        LinkedinLink = reader["LinkedinLink"].ToString(),
                                        PerHourRate = reader["PerHourRate"] != DBNull.Value ? (int)reader["PerHourRate"] : 0,
                                        WorkingHours = reader["WorkingHours"] != DBNull.Value ? (int)reader["WorkingHours"] : 0,
                                        CardId = reader["CardId"] != DBNull.Value ? (int)reader["CardId"] : (int?)null,
                                        UserId = Convert.ToInt32(reader["UserId"]),
                                        UserEmail = reader["UserEmail"].ToString(),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"].ToString(),
                                        // Adding Proposal Details
                                        ProposalId = Convert.ToInt32(reader["ProposalId"]),
                                        ProposalDescription = reader["ProposalDescription"].ToString(),
                                        ProposalBid = reader["ProposalBid"] != DBNull.Value ? (decimal)reader["ProposalBid"] : 0,
                                        ProposalStatus = reader["ProposalStatus"].ToString(),
                                        CompletionTime = reader["CompletionTime"].ToString(),
                                        JobId = Convert.ToInt32(reader["JobId"]),
                                        CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                        UpdatedOn = reader.IsDBNull(reader.GetOrdinal("UpdatedOn")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedOn"))
                                    };

                                    freelancerProposals.Add(freelancerProposal);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error reading from database: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
            }

            return freelancerProposals;
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

        public void UpdateProposalStatus(int proposalId, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "UPDATE Proposal SET ProposalStatus = @Status WHERE ProposalId = @ProposalId";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@ProposalId", proposalId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        Console.WriteLine("Proposal Updated");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
