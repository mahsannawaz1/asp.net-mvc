using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketPlace.Controllers
{
    public class FreelancerController : Controller
    {
        private readonly IFreelancerRepository _freelancerRepository;

        public FreelancerController(IFreelancerRepository freelancerRepository)
        {
            _freelancerRepository = freelancerRepository;
        }


        [HttpGet]
        public IActionResult ShowAllJobs()
        {

            List<Job> jobs = new List<Job>(); // Initialize the list
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;

            try
            {
             
                jobs = _freelancerRepository.ShowAllJobs();
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


        [Route("Freelancer/JobDetails/{id}")]
        public IActionResult JobDetails(int id)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            var data = _freelancerRepository.JobDetails(id);
            var details = Tuple.Create(data.job, data.client, data.user);
            ViewData["details"] = details;
            return View(); 
        }

        public IActionResult ShowAllProposals(int id)
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
            int freelancerId = _freelancerRepository.GetFreelancerIdByEmail(email);
            if (freelancerId == -1) {
                return RedirectToAction("Index", "Home");
            }
            List<Proposal> proposals = _freelancerRepository.GetProposalsForFreelancer(freelancerId);
            ViewData["proposals"] = proposals;
            return View();
        }

        [Route("Freelancer/ProposalDetails/{id}")]
        public IActionResult ProposalDetails(int id)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            Console.WriteLine(id);
            Proposal proposal = _freelancerRepository.ProposalDetails(id);           
            ViewData["proposal"] = proposal;
            return View();
        }
    }
}
