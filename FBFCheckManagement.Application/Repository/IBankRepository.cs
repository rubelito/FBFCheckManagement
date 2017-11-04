using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.Repository
{
    public interface IBankRepository
    {
        void AddBank(Bank bank);
        List<Bank> GetAllBanks();
        Bank GetBankById(int id);
        Bank GetBankByName(string name);
        void EditBank(Bank bankToEdit);
        bool IsSuccess { get; }
        string StatusMessage { get; }
    }
}