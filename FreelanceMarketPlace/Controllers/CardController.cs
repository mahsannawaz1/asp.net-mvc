using Microsoft.AspNetCore.Mvc;
using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Controllers
{
    public class CardController : Controller
    {
        private readonly ICardRepository _cardRepository;

        public CardController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpGet]
        public IActionResult GetCard(int cardId)
        {
          
            return View();
        }

        [HttpPost]
        public IActionResult AddCard(Card card)
        {
       
            return View();
        }

        [HttpPost]
        public IActionResult UpdateCard(Card card)
        {
          
            return View();
        }

        [HttpDelete]
        public IActionResult DeleteCard(int cardId)
        {
            return View();
        }
    }
}
