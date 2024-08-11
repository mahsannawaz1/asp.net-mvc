using FreelanceMarketPlace.Models.Entities;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface ISkillRepository
    {
        public void AddSkill(Skill skill);
        public void AddFreelancerSkill(int freelancerId, Skill skill);
        public void AddJobSkill(int jobId, Skill skill);
        public void RemoveFreelancerSkill(int freelancerId, Skill skill);
        public void RemoveJobSkill(int jobId, Skill skill);
        public Skill GetSkill(int skillId); // Optional: Retrieve a specific skill
        public List<Skill> GetAllSkills(); // Optional: Retrieve all skills

        public List<Skill> GetAllFreelancerSkills(); // Optional: Retrieve all freelancer skills

        public List<Skill> GetAllJobSkills(); // Optional: Retrieve all job skills
    }
}
