using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void CreateJob(Job job)
        {
      
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job), "Job object is null");
            }

            try
            {
                int JobId;
                // Define the query for inserting a new job
                string insertQuery = @"
            INSERT INTO Job (JobDescription, JobBudget, CreatedOn, UpdatedOn, JobLevel, ClientId, CompletionTime)
            OUTPUT INSERTED.JobId
            VALUES (@JobDescription, @JobBudget, @CreatedOn, @UpdatedOn, @JobLevel, @ClientId, @CompletionTime)";
                using (SqlConnection connect = new SqlConnection(ConnectionString))
                {
                    connect.Open();

                    // Use SqlCommand to execute the insert query
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connect))
                    {
                        // Add parameters for the job
                        cmd.Parameters.AddWithValue("@JobDescription", job.JobDescription);
                        cmd.Parameters.AddWithValue("@JobBudget", job.JobBudget);
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@JobLevel", job.JobLevel);
                        cmd.Parameters.AddWithValue("@ClientId", job.ClientId);
                        cmd.Parameters.AddWithValue("@CompletionTime", job.CompletionTime);

                        // Execute the query to insert the job into the database

                        JobId = (int)cmd.ExecuteScalar();
                        
                        Console.WriteLine("Job Created!");
                    }                
                    if ( job.Skills.Count > 0)
                    {         
                        
                        // Retrieve existing skills and get their IDs
                        var jobTuples = new (int Id, string Name)[job.Skills.Count]; 

                        for (int i = 0; i < job.Skills.Count; i++) 
                        {
                            Console.WriteLine(job.Skills[i]);
                            int skillId = GetOrCreateSkillId(job.Skills[i], connect);                           
                            jobTuples[i].Id = skillId;
                            jobTuples[i].Name = job.Skills[i];
                        }

                        for (int i = 0; i < job.Skills.Count; i++)
                        {
                            int skillId = jobTuples[i].Id;
                            if (!JobSkillExists(JobId, skillId, connect))
                            {
                                string insertJobSkillQuery = @"
                                    INSERT INTO JobSkill (JobId, SkillId)
                                    VALUES (@JobId, @SkillId)";

                                using (SqlCommand cmd = new SqlCommand(insertJobSkillQuery, connect))
                                {
                                    cmd.Parameters.Add("@JobId", SqlDbType.Int).Value = JobId;
                                    cmd.Parameters.Add("@SkillId", SqlDbType.Int).Value = skillId;

                                    cmd.ExecuteNonQuery();
                                }
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
                Console.WriteLine($"An error occurred: {ex}");
                throw new Exception("An error occurred while creating the job", ex);
            }
        }


        public (Job job, Client client, Users user, List<FreelancerProposals> proposalWithFreelancer) GetJob(int jobId)
        {
            Job job = new Job();
            Client client = new Client();
            Users user = new Users();
            List<FreelancerProposals> proposalWithFreelancer = new List<FreelancerProposals>();

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
                proposalWithFreelancer = GetFreelancersAndProposalsByJobId(jobId, connection);
            }
            
            // Return the job, client, and user as a tuple
            return (job, client, user, proposalWithFreelancer);
        }

        public void UpdateJob(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job), "Job object is null");
            }

            try
            {
                using (SqlConnection connect = new SqlConnection(ConnectionString))
                {
                    connect.Open();

                    // Define the query for updating the job
                    string updateQuery = @"
            UPDATE Job
            SET 
                JobDescription = @JobDescription,
                JobBudget = @JobBudget,
                UpdatedOn = @UpdatedOn,
                JobLevel = @JobLevel,
                CompletionTime = @CompletionTime
            WHERE JobId = @JobId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, connect))
                    {
                        // Add parameters for the job
                        cmd.Parameters.AddWithValue("@JobDescription", job.JobDescription);
                        cmd.Parameters.AddWithValue("@JobBudget", job.JobBudget);
                        cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@JobLevel", job.JobLevel);
                        cmd.Parameters.AddWithValue("@CompletionTime", job.CompletionTime);
                        cmd.Parameters.AddWithValue("@JobId", job.JobId);

                        // Execute the query to update the job in the database
                        cmd.ExecuteNonQuery();

                        Console.WriteLine("Job Updated!");
                    }

                    // Now update the skills if any are provided
                    if (job.Skills.Count > 0)
                    {
                        // Retrieve existing skills and get their IDs
                        var jobTuples = new (int Id, string Name)[job.Skills.Count];

                        for (int i = 0; i < job.Skills.Count; i++)
                        {
                            Console.WriteLine(job.Skills[i]);
                            int skillId = GetOrCreateSkillId(job.Skills[i], connect);  // Get existing skill or create if not present
                            jobTuples[i].Id = skillId;
                            jobTuples[i].Name = job.Skills[i];
                        }

                        // Now we need to update the JobSkill table for this job
                        for (int i = 0; i < job.Skills.Count; i++)
                        {
                            int skillId = jobTuples[i].Id;
                            if (!JobSkillExists(job.JobId, skillId, connect))
                            {
                                string insertJobSkillQuery = @"
                        INSERT INTO JobSkill (JobId, SkillId)
                        VALUES (@JobId, @SkillId)";

                                using (SqlCommand cmd = new SqlCommand(insertJobSkillQuery, connect))
                                {
                                    cmd.Parameters.Add("@JobId", SqlDbType.Int).Value = job.JobId;
                                    cmd.Parameters.Add("@SkillId", SqlDbType.Int).Value = skillId;

                                    cmd.ExecuteNonQuery();
                                }
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
                Console.WriteLine($"An error occurred: {ex}");
                throw new Exception("An error occurred while updating the job", ex);
            }
        }

        public void DeleteJob(int jobId)
        {
           
            // SQL queries for deleting data
            string deleteProposalsQuery = "DELETE FROM Proposal WHERE JobId = @JobId;";
            string deleteJobSkillsQuery = "DELETE FROM JobSkill WHERE JobId = @JobId;";
            string deleteJobQuery = "DELETE FROM Job WHERE JobId = @JobId;";

            // Use a transaction to ensure data integrity
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete proposals related to the job
                        using (SqlCommand deleteProposalsCmd = new SqlCommand(deleteProposalsQuery, connection, transaction))
                        {
                            deleteProposalsCmd.Parameters.AddWithValue("@JobId", jobId);
                            deleteProposalsCmd.ExecuteNonQuery();
                        }

                        // Delete job skills related to the job
                        using (SqlCommand deleteJobSkillsCmd = new SqlCommand(deleteJobSkillsQuery, connection, transaction))
                        {
                            deleteJobSkillsCmd.Parameters.AddWithValue("@JobId", jobId);
                            deleteJobSkillsCmd.ExecuteNonQuery();
                        }

                        // Delete the job itself
                        using (SqlCommand deleteJobCmd = new SqlCommand(deleteJobQuery, connection, transaction))
                        {
                            deleteJobCmd.Parameters.AddWithValue("@JobId", jobId);
                            deleteJobCmd.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                        Console.WriteLine("Job and related data deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction.Rollback();
                        Console.WriteLine($"Error occurred: {ex.Message}");
                    }
                }
            }
        }

        public List<Job> ShowAllJobs(int clientId)
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
                FROM Job
                WHERE ClientId = @ClientId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        // Add parameter for ClientId
                        cmd.Parameters.AddWithValue("@ClientId", clientId);

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
                    if(jobs.Count > 0)
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


        public int GetClientIdByEmail(string email)
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

                // If UserId is found, fetch the ClientId from the Client table using the UserId as a foreign key
                if (userId != -1)
                {
                    string clientQuery = "SELECT ClientId FROM Client WHERE UserId = @UserId";
                    using (SqlCommand cmd = new SqlCommand(clientQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                clientId = Convert.ToInt32(reader["ClientId"]);
                            }
                        }
                    }
                }
            }

            return clientId; // Return the found ClientId or -1 if no client was found
        }

        public FreelancerProposals GetProposalByIdOnJob(int proposalId)
        {
            FreelancerProposals proposal = new FreelancerProposals();
            string query = @"
            SELECT 
                f.FreelancerId, u.UserId, u.UserEmail, u.FirstName, u.LastName,
                p.ProposalId, p.ProposalDescription, p.ProposalBid, 
                p.CompletionTime, p.ProposalStatus, p.JobId, 
                p.CreatedOn, p.UpdatedOn
            FROM Freelancer f
            INNER JOIN Users u ON f.UserId = u.UserId
            INNER JOIN Proposal p ON f.FreelancerId = p.FreelancerId
            WHERE p.ProposalId = @ProposalId";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProposalId", proposalId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        proposal = new FreelancerProposals
                        {
                            FreelancerId = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            UserEmail = reader.GetString(2),
                            FirstName = reader.GetString(3),
                            LastName = reader.GetString(4),
                            ProposalId = reader.GetInt32(5),
                            ProposalDescription = reader.GetString(6),
                            ProposalBid = reader.GetDecimal(7),
                            CompletionTime = reader.IsDBNull(8) ? null : reader.GetString(8),
                            ProposalStatus = reader.GetString(9),
                            JobId = reader.GetInt32(10),
                            CreatedOn = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                            UpdatedOn = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                        };
                    }
                }
            }


            return proposal;
        }

        private bool JobSkillExists(int jobId, int skillId, SqlConnection connection)
        {
            string checkJobSkillQuery = @"
            SELECT COUNT(*)
            FROM JobSkill
            WHERE JobId = @JobId AND SkillId = @SkillId";

            using (SqlCommand cmd = new SqlCommand(checkJobSkillQuery, connection))
            {
                cmd.Parameters.Add("@JobId", SqlDbType.Int).Value = jobId;
                cmd.Parameters.Add("@SkillId", SqlDbType.Int).Value = skillId;

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }


        private int GetOrCreateSkillId(string skillName, SqlConnection connection)
        {
            // Check if the skill exists
            string selectSkillQuery = "SELECT SkillId FROM Skill WHERE SkillName = @Name";
            using (SqlCommand cmd = new SqlCommand(selectSkillQuery, connection))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = skillName;

                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return (int)result;
                }
            }

            // Skill does not exist, so insert it
            string insertSkillQuery = "INSERT INTO Skill (SkillName) OUTPUT INSERTED.SkillId VALUES (@Name)";
            using (SqlCommand cmd = new SqlCommand(insertSkillQuery, connection))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = skillName;

                return (int)cmd.ExecuteScalar();
            }
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

        public List<FreelancerProposals> GetFreelancersAndProposalsByJobId(int jobId, SqlConnection connection)
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
            F.UserId,
            U.UserEmail,
            U.FirstName,
            U.LastName,
            P.ProposalId,
            P.ProposalDescription,
            P.ProposalBid,
            P.CompletionTime,
            P.JobId,
            P.ProposalStatus,
            P.CreatedOn,
            P.UpdatedOn
        FROM 
            Freelancer F
        JOIN 
            Users U ON F.UserId = U.UserId
        JOIN 
            Proposal P ON P.FreelancerId = F.FreelancerId
        WHERE 
            P.JobId = @JobId";

           
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@JobId", jobId);

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
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    UserEmail = reader["UserEmail"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    // Adding Proposal Details
                                    ProposalId = Convert.ToInt32(reader["ProposalId"]),
                                    ProposalDescription = reader["ProposalDescription"].ToString(),
                                    ProposalStatus = reader["ProposalStatus"].ToString(),
                                    ProposalBid = reader["ProposalBid"] != DBNull.Value ? (decimal)reader["ProposalBid"] : 0,
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
                        Console.WriteLine(ex.Message);
                    }
                }
            return freelancerProposals;
       
        }
    }
}
