using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void AddCard(Card card)
        {
            // Implementation to add a new card
        }

        public void DeleteCard(int cardId)
        {
            // Implementation to delete a card
        }

        public void UpdateCard(Card card)
        {
            // Implementation to update a card
        }

        public Card GetCard(int cardId)
        {
            // Implementation to retrieve a specific card
            return new Card(); // Replace with actual data retrieval logic
        }

    }
}
