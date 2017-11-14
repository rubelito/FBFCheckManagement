using System;

namespace FBFCheckManagement.Application.DTO
{
    public class YearMonthInfo
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public bool ShouldFilterByBank { get; set; }
        public bool ShouldFilterByStatus { get; set; }

        public long BankId { get; set; }
        public CheckFlag Flag { get; set; }
    }
}