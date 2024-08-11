using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Controllers
{
    public class SkillController : Controller
    {
        private readonly ISkillRepository _skillRepository;

        public SkillController(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        [HttpPost]
        public IActionResult AddSkill(Skill skill)
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult AddFreelancerSkill(int freelancerId, Skill skill)
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddJobSkill(int jobId, Skill skill)
        {
            return View();
        }

        [HttpDelete]
        public IActionResult RemoveFreelancerSkill(int freelancerId, Skill skill)
        {
            return View();
        }

        [HttpDelete]
        public IActionResult RemoveJobSkill(int jobId, Skill skill)
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetSkill(int skillId)
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllSkills()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetFreelancerSkills(int freelancerId)
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetJobSkills(int jobId)
        {
            return View();
        }
    }
}
