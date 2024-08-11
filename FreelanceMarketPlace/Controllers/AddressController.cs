using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressRepository _addressRepository;

        public AddressController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet]
        public IActionResult GetAddress(int addressId)
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult AddAddress(Address address)
        {
            return View();
        }

        [HttpPatch]
        public IActionResult UpdateAddress(Address address)
        {
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteAddress(int addressId)
        {
            return View();
        }

    }
}
