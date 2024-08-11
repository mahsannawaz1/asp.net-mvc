using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Controllers
{
    public class RatingController : Controller
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        [HttpGet]
        public ViewResult GetRatingsForUser(int userId)
        {
           
            return View();
        }

        [HttpGet]
        public IActionResult GetRating(int ratingId)
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult AddRating(Rating rating)
        {
          
            return View();
        }

        [HttpPost]
        public IActionResult UpdateRating(Rating rating)
        {
           
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteRating(int ratingId)
        {

            return View();
        }
    }
}
