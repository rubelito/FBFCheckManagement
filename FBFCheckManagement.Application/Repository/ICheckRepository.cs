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
        List<Check> GetChecksByMonth(int year, int month);
        Check GetCheckByNumber(string checkNumber);
        List<Check> GetChecksByDateRange(DateTime from, DateTime to);
        CheckPagingResult GetCheckWithPaging(CheckPagingRequest request);
    }
}