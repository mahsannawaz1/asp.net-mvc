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
            return View();
        }

        [HttpGet]
        public ViewResult ShowAllProposalsOnJob(int JobId)
        {
            return View();
        }

        [HttpPost]
        [Route("SendProposal/{jobId}")]
        public IActionResult SendProposal(Proposal proposal)
        {
            Console.WriteLine($"kING");
            var currentUser = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            // Pass the user info to the view
            ViewBag.CurrentUser = currentUser;

            string email = HttpContext.Request.Cookies["AuthToken"];
            int FreelancerId = _proposalRepository.GetFreelancerIdByEmail(email);

            Console.WriteLine(FreelancerId);
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
            var currentUser = HttpContext.Request.Cookies.ContainsKey("AuthToken");
            // Pass the user info to the view
            ViewBag.CurrentUser = currentUser;
            ViewData["jobId"] = jobId;
            return View();
        }

        [HttpGet]
        public IActionResult GetProposal(int porposalId)
        {
            return View();
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
