using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;
using FreelanceMarketPlace.Models.Repositories;
using System.Data;
using Newtonsoft.Json;

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
            var currentUser = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            // Pass the user info to the view
            ViewBag.CurrentUser = currentUser;
            return View();
        }

        [HttpPost]
        public IActionResult CreateJob(Job job)
        {
            string email = HttpContext.Request.Cookies["AuthToken"];
            var currentUser = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            // Pass the user info to the view
            ViewBag.CurrentUser = currentUser;

            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {

                if (job != null)
                {
                    
                    int ClientId = _jobRepository.GetClientIdByEmail(email);
                    Console.WriteLine(ClientId);
                    if (ClientId == -1) {
                        return RedirectToAction("Index", "Home");
                    }
                    List<string> skills = JsonConvert.DeserializeObject<List<string>>(job.Skills[0]);
                    
                    Job newJob = new Job
                    {
                       JobDescription = job.JobDescription,
                       JobBudget = job.JobBudget,                     
                       JobLevel = job.JobLevel,
                       CompletionTime = job.CompletionTime,
                       ClientId = ClientId,
                       Skills = skills,
                    };


                    _jobRepository.CreateJob(newJob);
                    return RedirectToAction("ShowAllJobs", "Job");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

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

        [Route("Job/GetJob/{id}")]
        public IActionResult GetJob(int id)
        {
            var currentUser = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            // Pass the user info to the view
            ViewBag.CurrentUser = currentUser;
            var data = _jobRepository.GetJob(id);
            var details = Tuple.Create(data.job, data.client, data.user,data.proposalWithFreelancer);
            ViewData["details"] = details;
            return View();
        }

        [HttpGet]
        public IActionResult ShowAllJobs()
        {
            List<Job> jobs = new List<Job>(); // Initialize the list
            var currentUser = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            ViewBag.CurrentUser = currentUser;

            string email = HttpContext.Request.Cookies["AuthToken"];
            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                int clientId = _jobRepository.GetClientIdByEmail(email);

                if (clientId == -1)
                {
                    return RedirectToAction("Index", "Home");
                }

                jobs = _jobRepository.ShowAllJobs(clientId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                // Log the exception details
                Console.WriteLine($"Error: {ex.Message}");
            }

            
            ViewData["jobs"] = jobs;
            return View();
        }


        [HttpDelete]
        public IActionResult DeleteJob(int jobId)
        {
          
            return View();
        }
    }
}
