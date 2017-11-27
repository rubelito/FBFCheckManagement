using System;
using System.Collections.Generic;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;

namespace FBFCheckManagement.Application.Repository
{
    public interface ICheckRepository
    {
        void Add(Check check);
        void Update(Check check);
        void Delete(long id);
        Check GetCheckById(long id);
        List<Check> GetChecksByMonth(YearMonthInfo paramInfo);
        Check GetCheckByNumber(string checkNumber);
        List<Check> GetChecksByDateRange(DateTime from, DateTime to);
        List<Check> GetChecksByDateRangeWithDepartmentId(DateTime from, DateTime to, long deptId);
        List<Check> GetChecksByDateRangeWithBankId(DateTime from, DateTime to, long bankId);
        CheckPagingResult GetCheckWithPaging(CheckPagingRequest request);
        bool IsSuccess { get; }
        string ErrorMessage { get; }
    }
}