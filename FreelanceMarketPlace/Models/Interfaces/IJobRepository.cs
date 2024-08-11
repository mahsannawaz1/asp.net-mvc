using FreelanceMarketPlace.Models.Entities;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IJobRepository
    {
        public void CreateJob(Job job);
        public Job GetJob(int jobId);
        public void UpdateJob(Job job);
        public void DeleteJob(int jobId);
        public List<Job> ShowAllJobs();
    }
}
