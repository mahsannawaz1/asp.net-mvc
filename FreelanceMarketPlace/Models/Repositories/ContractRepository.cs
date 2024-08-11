using FreelanceMarketPlace.Models.Entities;
using FreelanceMarketPlace.Models.Interfaces;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FreelanceMarketPlace;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void AddContract(Contract contract)
        {
            // Implementation to add a contract
        }

        public void DeleteContract(int contractId)
        {
            // Implementation to delete a contract
        }

        public void CompleteContract(int contractId)
        {
            // Implementation to mark a contract as complete
        }

        public Contract GetContract(int contractId)
        {
            // Implementation to get a specific contract
            return new Contract(); // Replace with actual data retrieval logic
        }

        public List<Contract> GetAllContracts()
        {
            // Implementation to get all contracts
            return new List<Contract>(); // Replace with actual data retrieval logic
        }
    }
}
