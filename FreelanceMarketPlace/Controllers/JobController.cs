using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        [HttpGet]
        public ViewResult CreateJob()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateJob(Job job)
        {
          
            return View();
        }

        [HttpGet]
        public ViewResult UpdateJob(int jobId)
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult UpdateJob(Job job)
        {
           
            return View();
        }

        [HttpGet]
        public ViewResult GetJob(int jobId)
        {
           
            return View();
        }

        [HttpGet]
        public ViewResult ShowAllJobs()
        {
           
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteJob(int jobId)
        {
          
            return View();
        }
    }
}
