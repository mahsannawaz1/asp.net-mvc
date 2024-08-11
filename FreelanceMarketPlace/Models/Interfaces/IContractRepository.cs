using FreelanceMarketPlace.Models.Entities;
using System.Collections.Generic;

namespace FreelanceMarketPlace.Models.Interfaces
{
    public interface IContractRepository
    {
        void AddContract(Contract contract);
        void DeleteContract(int contractId);
        void CompleteContract(int contractId);
        Contract GetContract(int contractId);
        List<Contract> GetAllContracts();
    }
}
