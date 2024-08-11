using FreelanceMarketPlace.Models.Entities;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class JobRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void CreateJob(Job job)
        {
            // Implementation for creating a job
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

        public List<Job> ShowAllJobs()
        {
            List<Job> jobs = new List<Job>();
            // Example dummy data (replace with actual data retrieval logic)
            return jobs;
        }
    }
}
