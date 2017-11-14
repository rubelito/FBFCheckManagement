using System;
using FBFCheckManagement.Application.DTO;

namespace FBFCheckManagement.Application.Report
{
    public class ReportParameter
    {
        public ReportType Type { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
     
        public DateTime Day { get; set; }

        public CheckFlag CheckFlag { get; set; }
        
        public bool ShouldFilterByBank { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
    }

    public enum ReportType
    {
        Daily,
        Weekly
    }
}