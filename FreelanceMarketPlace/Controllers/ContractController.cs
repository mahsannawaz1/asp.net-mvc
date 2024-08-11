using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractRepository _contractRepository;

        public ContractController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        [HttpGet]
        public ViewResult GetAllContracts()
        {
           
            return View();
        }

        [HttpGet]
        public IActionResult GetContract(int contractId)
        {
          
            return View();
        }

        [HttpPost]
        public IActionResult AddContract(Contract contract)
        {
          
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteContract(int contractId)
        {
          
            return View();
        }

        [HttpPatch]
        public IActionResult CompleteContract(int contractId)
        {
           
            return View();
        }
    }
}
