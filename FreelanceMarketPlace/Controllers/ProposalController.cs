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
        public IActionResult SendProposal(ProposalController proposal)
        {
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
