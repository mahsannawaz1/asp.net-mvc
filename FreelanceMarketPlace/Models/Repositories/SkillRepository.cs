using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void AddSkill(Skill skill)
        {
            // Implementation to add a new skill
        }

        public void AddFreelancerSkill(int freelancerId, Skill skill)
        {
            // Implementation to add a skill to a freelancer
        }

        public void AddJobSkill(int jobId, Skill skill)
        {
            // Implementation to add a skill to a job
        }

        public void RemoveFreelancerSkill(int freelancerId, Skill skill)
        {
            // Implementation to remove a skill from a freelancer
        }

        public void RemoveJobSkill(int jobId, Skill skill)
        {
            // Implementation to remove a skill from a job
        }

        public Skill GetSkill(int skillId)
        {
            // Implementation to retrieve a specific skill
            return new Skill(); 
        }

        public List<Skill> GetAllSkills()
        {
            // Implementation to retrieve all skills
            return new List<Skill>(); 
        }


        public List<Skill> GetAllFreelancerSkills()
        {
            // Implementation to retrieve all freelancer skills
            return new List<Skill>(); 
        }

        public List<Skill> GetAllJobSkills()
        {
            // Implementation to retrieve all job skills
            return new List<Skill>(); 
        }
    }
}
