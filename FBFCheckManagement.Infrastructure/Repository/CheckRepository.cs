﻿using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Infrastructure.EntityFramework;

namespace FBFCheckManagement.Infrastructure.Repository
{
    public class CheckRepository : ICheckRepository
    {
        private readonly FBFDbContext _context;

        public CheckRepository(IDatabaseType databaseType)
        {
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new FBFDbContext(databaseType);
        }

        public void Add(Check check){
            Bank b = _context.Banks.FirstOrDefault(f => f.Id == check.Bank.Id);
            check.Bank = b;
            _context.Checks.Add(check);
            _context.SaveChanges();
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
        }

        public void Delete(long id){
            Check cToRemove = _context.Checks.FirstOrDefault(c => c.Id == id);
            _context.Checks.Remove(cToRemove);
            _context.SaveChanges();
        }

        public Check GetCheckById(long id){
            return _context.Checks.FirstOrDefault(c => c.Id == id);
        }

        public List<Check> GetChecksByMonth(int year, int month){
            return
                _context.Checks.Where(c => c.DateIssued.Value.Year == year && c.DateIssued.Value.Month == month)
                    .ToList();
        }

        public Check GetCheckByNumber(string checkNumber){
            return _context.Checks.FirstOrDefault(c => c.CheckNumber == checkNumber);
        }

        public List<Check> GetChecksByDateRange(DateTime from, DateTime to){
            return
                _context.Checks.Where(
                    c => c.DateIssued.HasValue && c.DateIssued.Value >= from && c.DateIssued.Value <= to).ToList();
        }

        //The library Devart DotConnect for SQLite has minor bugs when arranging data with skip and take.
        //Basically, what happens is order by is not being generated in SQL using Skip and take.
        //My work around is to create SQL by hand.

        public CheckPagingResult GetCheckWithPaging(CheckPagingRequest r)
        {
            CheckPagingResult result = new CheckPagingResult();
            string query = "Select * From Checks";
            string queryForCount = "Select Count(Id) From Checks";

            queryForCount = BuildCondition(r, queryForCount);

            result.TotalItems = _context.Database.SqlQuery<int>(queryForCount).First();            
            query = BuildCondition(r, query);
            query = BuildOrderBy(r, query);
            query = BuildPagination(r, query);
            
            result.Results = _context.Checks.SqlQuery(query).ToList();
            result.PageCount = result.TotalItems/r.PageSize;

            return result;
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
            if (s.IssuedDateFrom.HasValue && s.IssuedDateTo.HasValue)
            {
                DateTime addOneDay = s.IssuedDateTo.Value.AddDays(1);
                query = BuildWhereClause(query, "DateIssued", "Between",
                    "'" + s.IssuedDateFrom.Value.ToString("yyyy-MM-dd") + "' And '" + addOneDay.ToString("yyyy-MM-dd") + "'",
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
            if (s.CreatedDateFrom.HasValue && s.CreatedDateTo.HasValue)
            {
                DateTime addOneDay = s.CreatedDateTo.Value.AddDays(1);
                query = BuildWhereClause(query, "CreatedDate", "Between",
                    "'" + s.CreatedDateFrom.Value.ToString("yyyy-MM-dd") + "' And '" + addOneDay.ToString("yyyy-MM-dd") + "'",
                    shouldIncludeWhere);
            }

            return query;
        }

        private string BuildWhereClause(string query, string columnName,  
            string operation, string condition,bool shoulIncludeWhere){
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
            if (s.OrderBy == Application.DTO.OrderBy.CreatedDate){
                query = BuildOrderClause("CreatedDate", s.Order, query);
            }
            else if (s.OrderBy == Application.DTO.OrderBy.IssuedDate){
                query = BuildOrderClause("DateIssued", s.Order, query);
            }
            else if (s.OrderBy == Application.DTO.OrderBy.Amount){
                query = BuildOrderClause("Amount", s.Order, query);
            }
            else if (s.OrderBy == Application.DTO.OrderBy.CheckNumber){
                query = BuildOrderClause("CheckNumber", s.Order, query);
            }
            else if (s.OrderBy == Application.DTO.OrderBy.IssuedTo){
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
    }
}