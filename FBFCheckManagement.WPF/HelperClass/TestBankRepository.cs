using System.Collections.Generic;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;

namespace FBFCheckManagement.WPF.HelperClass
{
    public class TestBankRepository : IBankRepository
    {

        public void AddBank(Application.Domain.Bank bank)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<Application.Domain.Bank> GetAllBanks()
        {
            List<Bank> mybanks = new List<Bank>();
            mybanks.Add(new Bank() { BankName = "BDO", Id = 1 });
            mybanks.Add(new Bank() { BankName = "BPI", Id = 2 });
            mybanks.Add(new Bank() { BankName = "CITI", Id = 3 });
            mybanks.Add(new Bank() { BankName = "China Bank", Id = 4 });
            mybanks.Add(new Bank() { BankName = "Metro Bank", Id = 5 });

            return mybanks;
        }

        public Application.Domain.Bank GetBankById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Application.Domain.Bank GetBankByName(string name)
        {
            throw new System.NotImplementedException();
        }


        public string StatusMessage
        {
            get { throw new System.NotImplementedException(); }
        }


        public void EditBank(Bank bankToEdit)
        {
            throw new System.NotImplementedException();
        }


        public bool IsSuccess
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}