using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Infrastructure.EntityFramework;

namespace FBFCheckManagement.Infrastructure.Repository
{
    public class CheckRepository : ICheckRepository
    {
        private readonly FBFDbContext _context;

        private bool _isSuccess;
        private string _errorMessage = string.Empty;

        public CheckRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new FBFDbContext(databaseType);
        }

        public void Add(Check check){
            Bank b = _context.Banks.FirstOrDefault(f => f.Id == check.Bank.Id);
            bool isExist = b.Checks.Any(c => c.CheckNumber == check.CheckNumber);

            if (isExist){
                _isSuccess = false;
                _errorMessage = "Cannot Add Check: Check number " + check.CheckNumber + " already exist in " +
                                b.BankName;
            }
            else{
                check.Bank = b;
                _context.Checks.Add(check);
                _context.SaveChanges();
                _isSuccess = true;
            }
        }

        public void Update(Check check){
            var oldCheck = _context.Checks.FirstOrDefault(c => c.Id == check.Id);

            oldCheck.CheckNumber = check.CheckNumber;
            oldCheck.Amount = check.Amount;
            oldCheck.Bank = _context.Banks.FirstOrDefault(b => b.Id == check.Bank.Id);
            oldCheck.IssuedTo = check.IssuedTo;
            oldCheck.DateIssued = check.DateIssued;
            oldCheck.ModifiedDate = check.ModifiedDate;

            _context.SaveChanges();

            _isSuccess = true;
        }

        public void Delete(long id){
            Check cToRemove = _context.Checks.FirstOrDefault(c => c.Id == id);
            _context.Checks.Remove(cToRemove);
            _context.SaveChanges();
        }

        public Check GetCheckById(long id){
            return _context.Checks.FirstOrDefault(c => c.Id == id);
        }

        public List<Check> GetChecksByMonth(YearMonthInfo paramInfo){
            IQueryable<Check> query = _context.Checks;

            if (paramInfo.ShouldFilterByDepartment){
                query = query.Where(c => c.Bank.Department.Id == paramInfo.DepartmentId);
            }

            if (paramInfo.ShouldFilterByBank){
                query = query.Where(c => c.Bank.Id == paramInfo.BankId);
            }

            query = query.Where(
                c =>
                    (c.DateIssued.HasValue && c.HoldDate.HasValue == false &&
                     c.DateIssued.Value.Year == paramInfo.Year &&
                     c.DateIssued.Value.Month == paramInfo.Month)
                    ||
                    (c.HoldDate.HasValue && c.HoldDate.Value.Year == paramInfo.Year &&
                     c.HoldDate.Value.Month == paramInfo.Month));

            //query = ApplyCheckFlagQuery(query, paramInfo);

            return query.ToList();
        }

        //private IQueryable<Check> ApplyCheckFlagQuery(IQueryable<Check> query, YearMonthInfo param)
        //{
        //    IQueryable<Check> preQuery = query;

        //    if (param.Flag == CheckFlag.Funded)
        //    {
        //        preQuery = query.Where(c => c.IsFunded && !c.IsSettled);
        //    }
        //    if (param.Flag == CheckFlag.NotFunded)
        //    {
        //        preQuery = query.Where(c => !c.IsFunded && !c.IsSettled);
        //    }
        //    if (param.Flag == CheckFlag.Settled)
        //    {
        //        preQuery = query.Where(c => c.IsSettled);
        //    }

        //    return preQuery;
        //}

        public Check GetCheckByNumber(string checkNumber){
            return _context.Checks.FirstOrDefault(c => c.CheckNumber == checkNumber);
        }

        public List<Check> GetChecksByDateRange(DateTime from, DateTime to){
            return GetChecksWithRange(from, to).ToList();
        }

        public List<Check> GetChecksByDateRangeWithDepartmentId(DateTime from, DateTime to, long deptId){
            return GetChecksWithRange(from, to).Where(c => c.Bank.Department.Id == deptId).ToList();
        }

        public List<Check> GetChecksByDateRangeWithBankId(DateTime from, DateTime to, long bankId){
            return GetChecksWithRange(from, to).Where(c => c.Bank.Id == bankId).ToList();
        }

        private IQueryable<Check> GetChecksWithRange(DateTime from, DateTime to){
            return _context.Checks.Where(
                c =>
                    (c.DateIssued.HasValue && c.HoldDate.HasValue == false &&
                     c.DateIssued.Value >= from &&
                     c.DateIssued.Value <= to)
                    ||
                    (c.HoldDate.HasValue && c.HoldDate.Value >= from &&
                     c.HoldDate.Value <= to));
        }

        //The library Devart DotConnect for SQLite has minor bugs when arranging data with skip and take.
        //Basically, what happens is 'Order By' is not being generated in SQL using Skip and take.
        //My work around is to create SQL by hand.

        public CheckPagingResult GetCheckWithPaging(CheckPagingRequest r){
            CheckPagingResult result = new CheckPagingResult();
            string query = "Select * From Checks";
            string queryForCount = "Select Count(Checks.Id) From Checks";

            query = AddRelation(r, query);
            queryForCount = AddRelation(r, queryForCount);

            queryForCount = BuildCondition(r, queryForCount);

            result.TotalItems = _context.Database.SqlQuery<int>(queryForCount).First();
            query = BuildCondition(r, query);
            query = BuildOrderBy(r, query);
            query = BuildPagination(r, query);

            result.Results = _context.Checks.SqlQuery(query).ToList();
            result.PageCount = result.TotalItems/r.PageSize;

            return result;
        }

        private string AddRelation(CheckPagingRequest r, string query){
            SearchCriteria s = r.SearchCriteria;
            if (s.ShouldJoinTable){
                string partialQuery = string.Empty;
                partialQuery = query + " Inner join Banks on Checks.Bank_Id = Banks.Id";
                query = partialQuery + " Inner join Departments on Banks.Department_Id = Departments.Id";
            }

            return query;
        }

        private string BuildCondition(CheckPagingRequest r, string query){
            SearchCriteria s = r.SearchCriteria;
            bool shouldIncludeWhere = true;

            if (!string.IsNullOrEmpty(s.CheckNumber)){
                query = BuildWhereClause(query, "CheckNumber", "Like", "'%" + s.CheckNumber + "%'", shouldIncludeWhere);
                shouldIncludeWhere = false;
            }
            if (s.AmountFrom != 0 && s.AmountTo != 0){
                query = BuildWhereClause(query, "Amount", "Between", s.AmountFrom + " And " + s.AmountTo,
                    shouldIncludeWhere);
                shouldIncludeWhere = false;
            }
            if (s.IssuedDateFrom.HasValue && s.IssuedDateTo.HasValue){
                DateTime addOneDay = s.IssuedDateTo.Value.AddDays(1);
                query = BuildWhereClause(query, "DateIssued", "Between",
                    "'" + s.IssuedDateFrom.Value.ToString("yyyy-MM-dd") + "' And '" + addOneDay.ToString("yyyy-MM-dd") +
                    "'",
                    shouldIncludeWhere);
                shouldIncludeWhere = false;
            }
            if (s.SelectedBank != null){
                query = BuildWhereClause(query, "Bank_Id", "=", s.SelectedBank.Id.ToString(),
                    shouldIncludeWhere);
                shouldIncludeWhere = false;
            }
            if (!string.IsNullOrEmpty(s.IssuedTo)){
                query = BuildWhereClause(query, "IssuedTo", "Like", "'%" + s.IssuedTo + "%'",
                    shouldIncludeWhere);
                shouldIncludeWhere = false;
            }
            if (s.CreatedDateFrom.HasValue && s.CreatedDateTo.HasValue){
                DateTime addOneDay = s.CreatedDateTo.Value.AddDays(1);
                query = BuildWhereClause(query, "CreatedDate", "Between",
                    "'" + s.CreatedDateFrom.Value.ToString("yyyy-MM-dd") + "' And '" + addOneDay.ToString("yyyy-MM-dd") +
                    "'",
                    shouldIncludeWhere);
                shouldIncludeWhere = false;
            }
            if (s.ShouldJoinTable){
                query = BuildWhereClause(query, "Departments.Id", "=", Convert.ToString(s.SelectedDepartment.Id),
                    shouldIncludeWhere);
            }

            return query;
        }

        private string BuildWhereClause(string query, string columnName,
            string operation, string condition, bool shoulIncludeWhere){
            if (shoulIncludeWhere){
                query += " Where " + columnName + " " + operation + " " + condition;
            }
            else{
                query += " And " + columnName + " " + operation + " " + condition;
            }

            return query;
        }

        private string BuildOrderBy(CheckPagingRequest r, string query){

            SearchCriteria s = r.SearchCriteria;
            if (s.OrderBy == OrderBy.CreatedDate){
                query = BuildOrderClause("CreatedDate", s.Order, query);
            }
            else if (s.OrderBy == OrderBy.IssuedDate){
                query = BuildOrderClause("DateIssued", s.Order, query);
            }
            else if (s.OrderBy == OrderBy.Amount){
                query = BuildOrderClause("Amount", s.Order, query);
            }
            else if (s.OrderBy == OrderBy.CheckNumber){
                query = BuildOrderClause("CheckNumber", s.Order, query);
            }
            else if (s.OrderBy == OrderBy.IssuedTo){
                query = BuildOrderClause("IssuedTo", s.Order, query);
            }

            return query;
        }

        private string BuildOrderClause(string column, OrderByArrangement order, string query){
            if (order == OrderByArrangement.Ascending){
                query += " Order By " + column + " ASC";
            }
            else{
                query += " Order By " + column + " DESC";
            }

            return query;
        }

        private string BuildPagination(CheckPagingRequest r, string query){
            return query + " LIMIT " + r.PageSize + " OFFSET " + r.CurrentPage*r.PageSize;
        }

        public bool IsSuccess{
            get { return _isSuccess; }
        }

        public string ErrorMessage{
            get { return _errorMessage; }
        }
    }
}