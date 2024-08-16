using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.Data.SqlClient;
using System;

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
                // Define the query for inserting a new job
                string insertQuery = @"
            INSERT INTO Job (JobDescription, JobBudget, CreatedOn, UpdatedOn, JobLevel, ClientId,CompletionTime)
            VALUES (@JobDescription, @JobBudget, @CreatedOn, @UpdatedOn, @JobLevel, @ClientId,@CompletionTime)";
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
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Job Created!");
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
                throw new Exception("An error occurred while creating the job", ex);
            }
        }


        public Job GetJob(int jobId)
        {
            // Implementation for retrieving a single job
            return new Job();
        }

        public void UpdateJob(Job job)
        {
            // Implementation for updating a job
        }

        public void DeleteJob(int jobId)
        {
            // Implementation for deleting a job
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


    }
}
