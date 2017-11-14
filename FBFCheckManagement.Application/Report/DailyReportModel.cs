using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.DTO;

namespace FBFCheckManagement.Application.Report
{
    public class DailyReportModel
    {
        public DailyReportModel() {
            SectionsPerBank = new List<BankSection>();
        }

        public DateTime Day { get; set; }
        public DateTime DateGenerated { get; set; }

        public List<BankSection>  SectionsPerBank { get; set; }
        public CheckFlag CheckFlag { get; set; }

        public decimal TotalAmount {get { return SectionsPerBank.Sum(s => s.TotalAmount); }}

        public decimal TotalSettledAmount { get { return SectionsPerBank.Sum(s => s.SettledAmount); }}

        public decimal TotalRemaining { get { return SectionsPerBank.Sum(s => s.RemainingAmount); }}
    }
}