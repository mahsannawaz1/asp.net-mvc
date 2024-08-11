using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void AddAddress(Address address)
        {
            // Implementation to add a new address
        }

        public void UpdateAddress(Address address)
        {
            // Implementation to update an address
        }

        public void DeleteAddress(int addressId)
        {
            // Implementation to delete an address
        }

        public Address GetAddress(int addressId)
        {
            // Implementation to retrieve a specific address
            return new Address(); // Replace with actual data retrieval logic
        }
    }
}
