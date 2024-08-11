using FreelanceMarketPlace.Models.Entities;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IAddressRepository
    {
        void AddAddress(Address address);
        void UpdateAddress(Address address);
        void DeleteAddress(int addressId);
        Address GetAddress(int addressId); // Optional: Retrieve a specific address
    }
}
