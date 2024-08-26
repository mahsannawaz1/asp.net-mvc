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
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public IActionResult CreateJob(Job job)
        {
            string email = HttpContext.Request.Cookies["AuthToken"];
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;

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
        [Route("Job/UpdateJob/{jobId}")]
        public IActionResult UpdateJob(int jobId)
        {
            Console.WriteLine(jobId);
            string email = HttpContext.Request.Cookies["AuthToken"];
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            ViewBag.jobId = jobId;
            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult UpdateJob(Job job)
        {
            
            string email = HttpContext.Request.Cookies["AuthToken"];
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;

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
                    if (ClientId == -1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    List<string> skills = JsonConvert.DeserializeObject<List<string>>(job.Skills[0]);

                    Job newJob = new Job
                    {
                        JobId = job.JobId,
                        JobDescription = job.JobDescription,
                        JobBudget = job.JobBudget,
                        JobLevel = job.JobLevel,
                        CompletionTime = job.CompletionTime,
                        ClientId = ClientId,
                        Skills = skills,
                    };


                    _jobRepository.UpdateJob(newJob);
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

        [Route("Job/GetJob/{id}")]
        public IActionResult GetJob(int id)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            var data = _jobRepository.GetJob(id);
            var details = Tuple.Create(data.job, data.client, data.user,data.proposalWithFreelancer);
            ViewData["details"] = details;
            return View();
        }

        [HttpGet]
        public IActionResult ShowAllJobs(string levels = null, string sortBy = "")
        {
            List<Job> jobs = new List<Job>(); // Initialize the list
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;

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
                List<string> levelsList = new List<string>();
                if (!string.IsNullOrEmpty(levels))
                {
                    levelsList = JsonConvert.DeserializeObject<List<string>>(levels);
                }
                jobs = _jobRepository.ShowAllJobs(clientId,levelsList,sortBy);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                // Log the exception details
                Console.WriteLine($"Error: {ex.Message}");
            }



            // Check if the request is made via AJAX
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("~/Views/Shared/_AllJobs.cshtml", jobs); // Return the partial view with the filtered jobs
            }

            ViewData["jobs"] = jobs;
            return View();
        }


        [HttpDelete]
        public IActionResult DeleteJob(int jobId)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            if (!authToken) { 
                return RedirectToAction("Index", "Home");   
            }
           
            if(role == "client")
            {
                _jobRepository.DeleteJob(jobId);
            }
            return Json(new { success = true, message = "Job deleted successfully" });
        }

        [HttpGet]
        [Route("Job/GetProposal/{proposalId}")]
        public IActionResult GetProposal(int proposalId)
        {
            Console.WriteLine(proposalId);
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            FreelancerProposals proposal = _jobRepository.GetProposalByIdOnJob(proposalId);
            Console.WriteLine(proposal.FirstName);
            ViewData["proposal"] = proposal;
            return View();
        }
    }
}
