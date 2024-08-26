using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
namespace FreelanceMarketPlace.Models.Repositories
{
    public class FreelancerRepository : IFreelancerRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public List<Job> ShowAllJobs(List<string> levels, string sortBy)
        {
            List<Job> jobs = new List<Job>();
            try
            {
                // Open a connection to the database
                using (SqlConnection connect = new SqlConnection(ConnectionString))
                {
                    connect.Open();

                    // Define the query to get jobs for a specific client
                    string query = @"
                SELECT JobId, JobDescription, JobBudget, CreatedOn, UpdatedOn, JobLevel, ClientId, CompletionTime
                FROM Job";
                    if (levels != null && levels.Count > 0)
                    {
                        query += " WHERE JobLevel IN (" + string.Join(",", levels.Select((_, i) => $"@level{i}")) + ")";
                    }

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        query += " ORDER BY CreatedOn DESC"; // Ensure there's a space before ORDER BY
                    }


                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        // Add parameter for ClientId
                        if (levels != null && levels.Count > 0)
                        {
                            for (int i = 0; i < levels.Count; i++)
                            {
                                cmd.Parameters.AddWithValue($"@level{i}", levels[i]);
                            }
                        }

                        // Execute the query and read the results
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new Job object and populate it with data
                                Job job = new Job
                                {
                                    JobId = (int)reader["JobId"],
                                    JobDescription = reader["JobDescription"].ToString(),
                                    JobBudget = (decimal)reader["JobBudget"],
                                    CreatedOn = (DateTime)reader["CreatedOn"],
                                    UpdatedOn = (DateTime)reader["UpdatedOn"],
                                    JobLevel = reader["JobLevel"].ToString(),
                                    ClientId = (int)reader["ClientId"],
                                    CompletionTime = (string)reader["CompletionTime"]
                                };

                                // Add the Job object to the list
                                jobs.Add(job);
                                
                            }
                        }
                    }
                    if (jobs.Count > 0)
                    {
                        foreach (var job in jobs)
                        {
                            job.Skills = GetSkillsForJob(job.JobId, connect);
                        }

                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL-specific exceptions here
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("Database operation failed", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle general exceptions here
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new Exception("An error occurred while retrieving the jobs", ex);
            }

            // Return the list of jobs
            return jobs;
        }


        private List<string> GetSkillsForJob(int jobId, SqlConnection connection)
        {
            List<string> skills = new List<string>();

            string query = @"
            SELECT s.SkillName
            FROM JobSkill js
            JOIN Skill s ON js.SkillId = s.SkillId
            WHERE js.JobId = @JobId";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add("@JobId", SqlDbType.Int).Value = jobId;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        skills.Add(reader["SkillName"].ToString());
                    }
                }
            }

            return skills;
        }


        public (Job job, Client client, Users user) JobDetails(int jobId)
        {
            Job job = new Job();
            Client client = new Client();
            Users user = new Users();

            // SQL Query to get job, client, and user details
            string sql = @"
    SELECT j.JobId, j.JobDescription, j.JobBudget, j.JobStatus, j.JobLevel, j.CompletionTime, 
           j.CreatedOn, j.UpdatedOn, j.CompletedOn, 
           c.ClientId, c.AmountSpent,
           u.UserId, u.UserEmail, u.FirstName, u.LastName, u.Role, u.Phone
    FROM Job j
    JOIN Client c ON j.ClientId = c.ClientId
    JOIN Users u ON c.UserId = u.UserId
    WHERE j.JobId = @JobId;
    ";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@JobId", jobId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Populate Job object
                        job = new Job
                        {
                            JobId = reader.GetInt32(0),
                            JobDescription = reader.IsDBNull(1) ? null : reader.GetString(1),
                            JobBudget = reader.GetDecimal(2),
                            JobStatus = reader.GetString(3),
                            JobLevel = reader.GetString(4),
                            CompletionTime = reader.GetString(5),
                            CreatedOn = reader.GetDateTime(6),
                            UpdatedOn = reader.GetDateTime(7),
                        };

                        // Populate Client object
                        client = new Client
                        {
                            ClientId = reader.GetInt32(9),
                            AmountSpent = reader.IsDBNull(10) ? 0 : reader.GetDecimal(10),
                        };

                        // Populate User object
                        user = new Users
                        {
                            UserId = reader.GetInt32(11),
                            UserEmail = reader.GetString(12),
                            FirstName = reader.GetString(13),
                            LastName = reader.GetString(14),
                            Role = reader.GetString(15),
                            Phone = reader.IsDBNull(16) ? null : reader.GetString(16)
                        };
                    }
                }

                // Retrieve skills using a separate connection
                job.Skills = GetSkillsForJob(job.JobId, connection);
            }

            // Return the job, client, and user as a tuple
            return (job, client, user);
        }

        public List<Proposal> GetProposalsForFreelancer(int freelancerId)
        {
            List<Proposal> proposals = new List<Proposal>();

            try
            {
                // Open a connection to the database
                using (SqlConnection connect = new SqlConnection(ConnectionString))
                {
                    connect.Open();

                    // Define the query to get jobs for a specific client
                    string query = @"
                SELECT ProposalId,ProposalDescription,ProposalBid,ProposalStatus,CreatedOn,UpdatedOn,JobId
                FROM Proposal WHERE freelancerid = @FreelancerId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        // Add parameter for ClientId
                        cmd.Parameters.AddWithValue("@FreelancerId",freelancerId);

                        // Execute the query and read the results
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a new Proposal object and populate it with data
                                Proposal proposal = new Proposal
                                {
                                    ProposalId = (int)reader["ProposalId"],
                                    ProposalDescription = reader["ProposalDescription"].ToString(),
                                    ProposalBid = (decimal)reader["ProposalBid"],
                                    ProposalStatus = (string)reader["ProposalStatus"],
                                    CreatedOn = (DateTime)reader["CreatedOn"],
                                    UpdatedOn = (DateTime)reader["UpdatedOn"],
                                    JobId = (int)reader["JobId"],
                                };

                                // Add the Proposal object to the list
                                proposals.Add(proposal);
                            }
                        }
                    }
                   
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL-specific exceptions here
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("Database operation failed", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle general exceptions here
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new Exception("An error occurred while retrieving the proposals", ex);
            }

            // Return the list of proposals
            return proposals;
        }

        public int GetFreelancerIdByEmail(string email)
        {
            int userId = -1; // Assign a default value indicating no user found
            int freelancerId = -1; // Assign a default value indicating no client found

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
                                freelancerId = Convert.ToInt32(reader["FreelancerId"]);
                            }
                        }
                    }
                }
            }

            return freelancerId; // Return the found FreelancerId or -1 if no client was found
        }

        public Proposal ProposalDetails(int ProposalId)
        {
            Proposal proposal = new Proposal();

            try
            {
                // Open a connection to the database
                using (SqlConnection connect = new SqlConnection(ConnectionString))
                {
                    connect.Open();

                    // Define the query to get jobs for a specific client
                    string query = @"
                SELECT ProposalId,ProposalDescription,ProposalBid,ProposalStatus,CreatedOn,UpdatedOn,JobId
                FROM Proposal WHERE ProposalId = @ProposalId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        // Add parameter for ClientId
                        cmd.Parameters.AddWithValue("@ProposalId", ProposalId);

                        // Execute the query and read the results
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                proposal = new Proposal
                                {
                                    ProposalId = reader.GetInt32(reader.GetOrdinal("ProposalId")),
                                    ProposalDescription = reader.IsDBNull(reader.GetOrdinal("ProposalDescription")) ? null : reader.GetString(reader.GetOrdinal("ProposalDescription")),
                                    ProposalBid = reader.IsDBNull(reader.GetOrdinal("ProposalBid")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ProposalBid")),
                                    ProposalStatus = reader.IsDBNull(reader.GetOrdinal("ProposalStatus")) ? null : reader.GetString(reader.GetOrdinal("ProposalStatus")),
                                    CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                    UpdatedOn = reader.IsDBNull(reader.GetOrdinal("UpdatedOn")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),
                                    JobId = reader.IsDBNull(reader.GetOrdinal("JobId")) ? 0 : reader.GetInt32(reader.GetOrdinal("JobId")),
                                };
                            }
                        }
                    }

                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL-specific exceptions here
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw new Exception("Database operation failed", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle general exceptions here
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new Exception("An error occurred while retrieving the proposal", ex);
            }

            // Return the list of proposals
            return proposal;
        }

    }
}
