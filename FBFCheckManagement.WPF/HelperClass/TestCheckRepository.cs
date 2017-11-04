using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Repository;

namespace FBFCheckManagement.WPF.HelperClass
{
    public class TestCheckRepository : ICheckRepository
    {
        private List<Bank> mybanks;
        private List<Check> checks;

        public TestCheckRepository(){
            mybanks = new List<Bank>();
            checks = new List<Check>();
            CreateDummyData();
        }

        private void CreateDummyData(){
            mybanks.Add(new Bank() { BankName = "BDO", Id = 1 });
            mybanks.Add(new Bank() { BankName = "BPI", Id = 2 });
            mybanks.Add(new Bank() { BankName = "CITI", Id = 3 });
            mybanks.Add(new Bank() { BankName = "China Bank", Id = 4 });
            mybanks.Add(new Bank() { BankName = "Metro Bank", Id = 5 });

            checks.Add(new Check() { CheckNumber = "09983", Amount = 990, Id = 1, Bank = mybanks[4], IssuedTo = "Rubelito", DateIssued = new DateTime(2018, 2, 1), CreatedDate = new DateTime(2017, 6, 29) });
            checks.Add(new Check() { CheckNumber = "09982", Amount = 800, Id = 2, Bank = mybanks[1], IssuedTo = "Ruby", DateIssued = new DateTime(2018, 4, 2), CreatedDate = new DateTime(2017, 2, 1) });
            checks.Add(new Check() { CheckNumber = "09984", Amount = 123, Id = 3, Bank = mybanks[2], IssuedTo = "Bitong", DateIssued = new DateTime(2018, 5, 12), CreatedDate = new DateTime(2017, 7, 2) });
            checks.Add(new Check() { CheckNumber = "09985", Amount = 991, Id = 4, Bank = mybanks[3], IssuedTo = "Tongbits", DateIssued = new DateTime(2018, 2, 5), CreatedDate = DateTime.Now });
            checks.Add(new Check() { CheckNumber = "09986", Amount = 900000000, Id = 5, Bank = mybanks[1], IssuedTo = "LOL", DateIssued = new DateTime(2018, 12, 8), CreatedDate = new DateTime(2017, 7, 5) });
            checks.Add(new Check() { CheckNumber = "09988", Amount = 445, Id = 6, Bank = mybanks[2], IssuedTo = "100 First Division", DateIssued = new DateTime(2017, 8, 2), CreatedDate = new DateTime(2017, 4, 12) });
            checks.Add(new Check() { CheckNumber = "09283", Amount = 995, Id = 7, Bank = mybanks[3], IssuedTo = "Hahaha", DateIssued = new DateTime(2018, 3, 14), CreatedDate = new DateTime(2017, 8, 9) });
            checks.Add(new Check() { CheckNumber = "09989", Amount = 212, Id = 8, Bank = mybanks[2], IssuedTo = "hehehe", DateIssued = new DateTime(2018, 8, 18), CreatedDate = new DateTime(2017, 5, 11) });
            checks.Add(new Check() { CheckNumber = "09910", Amount = 879, Id = 9, Bank = mybanks[4], IssuedTo = "Mas", DateIssued = new DateTime(2018, 10, 9), CreatedDate = new DateTime(2017, 9, 29) });
            checks.Add(new Check() { CheckNumber = "09110", Amount = 15000, Id = 10, Bank = mybanks[4], IssuedTo = "Mouse", DateIssued = new DateTime(2018, 11, 28), CreatedDate = new DateTime(2017, 6, 28) });
            checks.Add(new Check() { CheckNumber = "02210", Amount = 135, Id = 11, Bank = mybanks[3], IssuedTo = "Trap", DateIssued = new DateTime(2018, 3, 19), CreatedDate = new DateTime(2017, 11, 19) });
        }

        public void Add(Application.Domain.Check check)
        {
            checks.Add(check);
        }

        public void Update(Application.Domain.Check check)
        {
            throw new System.NotImplementedException();
        }

        public Application.Domain.Check GetCheckById(long id){
            return checks.FirstOrDefault(c => c.Id == id);
        }

        public System.Collections.Generic.List<Application.Domain.Check> GetChecksByMonth(int year, int month)
        {
            throw new System.NotImplementedException();
        }

        public Application.Domain.Check GetCheckByNumber(string checkNumber)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<Application.Domain.Check> GetChecksByDateRange(System.DateTime from, System.DateTime to)
        {
            throw new System.NotImplementedException();
        }

        public CheckPagingResult GetCheckWithPaging(CheckPagingRequest r)
        {
            CheckPagingResult result = new CheckPagingResult();
            IEnumerable<Check> query = LoachChecks();

            query = BuildSearchQuery(r, query);
            query = OrderBy(r, query);

            result.TotalItems = query.Count();
            query = query.Skip(r.CurrentPage*r.PageSize).Take(r.PageSize);
            result.Results = query.ToList();
            result.PageCount = result.TotalItems/r.PageSize;

            return result;
        }

        private List<Check> LoachChecks(){
            return checks;
        }

        private IEnumerable<Check> BuildSearchQuery(CheckPagingRequest r, IEnumerable<Check> query){
            SearchCriteria s = r.SearchCriteria;

            if (!string.IsNullOrEmpty(s.CheckNumber))
                query = query.Where(q => q.CheckNumber.ToLower().Contains(s.CheckNumber.ToLower()));
            if (s.AmountFrom != 0 && s.AmountTo != 0)
                query = query.Where(q => q.Amount >= s.AmountFrom && q.Amount <= s.AmountTo);
            if (s.IssuedDateFrom.HasValue && s.IssuedDateTo.HasValue)
                query = query.Where(q => q.DateIssued >= s.IssuedDateFrom.Value && q.DateIssued <= s.IssuedDateTo.Value);
            if (s.SelectedBank != null)
                query = query.Where(q => q.Bank.Id == s.SelectedBank.Id);
            if (!string.IsNullOrEmpty(s.IssuedTo))
                query = query.Where(q => q.IssuedTo.ToLower().Contains(s.IssuedTo.ToLower()));
            if (s.CreatedDateFrom.HasValue && s.CreatedDateTo.HasValue){
                DateTime addOneDay = s.CreatedDateTo.Value.AddDays(1);
                query =
                    query.Where(q => q.CreatedDate >= s.CreatedDateFrom.Value && q.CreatedDate <= addOneDay);
            }

            return query;
        }

        private IEnumerable<Check> OrderBy(CheckPagingRequest r, IEnumerable<Check> query){
            SearchCriteria s = r.SearchCriteria;

            if (s.Order == OrderByArrangement.Ascending)
            {
                if (s.OrderBy == Application.DTO.OrderBy.CreatedDate)
                {
                    query = query.OrderBy(c => c.CreatedDate);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.IssuedDate){
                    query = query.OrderBy(c => c.DateIssued);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.Amount){
                    query = query.OrderBy(c => c.Amount);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.CheckNumber){
                    query = query.OrderBy(c => c.CheckNumber);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.IssuedTo){
                    query = query.OrderBy(c => c.IssuedTo);
                }
            }
            else if (s.Order == OrderByArrangement.Descending){
                if (s.OrderBy == Application.DTO.OrderBy.CreatedDate){
                    query = query.OrderByDescending(c => c.CreatedDate);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.IssuedDate){
                    query = query.OrderByDescending(c => c.DateIssued);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.Amount){
                    query = query.OrderByDescending(c => c.Amount);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.CheckNumber){
                    query = query.OrderByDescending(c => c.CheckNumber);
                }
                else if (s.OrderBy == Application.DTO.OrderBy.IssuedTo){
                    query = query.OrderByDescending(c => c.IssuedTo);
                }
            }

            return query;
        }


        public void Delete(long id)
        {
            throw new NotImplementedException();
        }
    }
}