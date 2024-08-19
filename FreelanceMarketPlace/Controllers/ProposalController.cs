using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using FreelanceMarketPlace.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketPlace.Controllers
{
    public class ProposalController : Controller
    {

        private readonly IProposalRepository _proposalRepository;
        public ProposalController(IProposalRepository proposalRepository)
        {
            _proposalRepository = proposalRepository;
        }

        [HttpGet]
        public ViewResult ShowAllSendProposals()
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;

            string email = HttpContext.Request.Cookies["AuthToken"];
            int FreelancerId = _proposalRepository.GetFreelancerIdByEmail(email);
            List<FreelancerProposals> proposals = _proposalRepository.ShowAllSendProposals(FreelancerId);
            ViewData["proposals"] = proposals;
            return View();
        }

        [HttpGet]
        public ViewResult ShowAllProposalsOnJob(int JobId)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        [Route("SendProposal/{jobId}")]
        public IActionResult SendProposal(Proposal proposal)
        {


            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;

            string email = HttpContext.Request.Cookies["AuthToken"];
            int FreelancerId = _proposalRepository.GetFreelancerIdByEmail(email);

            if (FreelancerId == -1)
            {
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine(proposal.JobId);
            Proposal newProposal = new Proposal
            {
                ProposalBid = proposal.ProposalBid,
                ProposalDescription = proposal.ProposalDescription,
                FreelancerId=FreelancerId,
                CompletionTime = proposal.CompletionTime,
                JobId = (int)proposal.JobId,
            };
            _proposalRepository.SendProposal(newProposal);
            return View();
        }


        [HttpGet]
        [Route("SendProposal/{jobId}")]
        public ActionResult SendProposal(int jobId)
        {
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            ViewData["jobId"] = jobId;
            return View();
        }

        [HttpGet]
        public IActionResult GetProposal(int porposalId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateStatus(int proposalId,string status)
        {
            Console.WriteLine($"Status: {status}");
            bool authToken = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            HttpContext.Request.Cookies.TryGetValue("Role", out string role);
            ViewBag.CurrentUser = authToken;
            ViewBag.Role = role;
            _proposalRepository.UpdateProposalStatus(proposalId, status);
            return Json(new { success = true, message = "Status updated successfully" });
        }

        [HttpPatch]
        public IActionResult UpdateProposal(Proposal proposal)
        {
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteProposal(int porposalId)
        {
            return View();
        }
    }
}
