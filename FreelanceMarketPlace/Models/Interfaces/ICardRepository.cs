using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface ICardRepository
    {
        void AddCard(Card card);
        void DeleteCard(int cardId);
        void UpdateCard(Card card);
        Card GetCard(int cardId); // Optional: Retrieve a specific card
    }
}
