using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;

namespace FBFCheckManagement.Application.Report
{
    public class BankSection
    {
        private DailyReportModel _parentReport;

        public BankSection(DailyReportModel parentReport){
            _parentReport = parentReport;
            Checks = new List<Check>();
        }

        public List<Check> Checks { get; set; }

        public List<Check> ChecksToDisplay{
            get{
                List<Check> toDisplay = new List<Check>();

                if (_parentReport.CheckFlag == CheckFlag.All){
                    toDisplay = Checks;
                }
                if (_parentReport.CheckFlag == CheckFlag.NotFunded){
                    toDisplay = Checks.Where(c => !c.IsFunded && !c.IsSettled).ToList();
                }
                if (_parentReport.CheckFlag == CheckFlag.Funded){
                    toDisplay = Checks.Where(c => c.IsFunded && !c.IsSettled).ToList();
                }
                if (_parentReport.CheckFlag == CheckFlag.Settled){
                    toDisplay = Checks.Where(c => c.IsSettled).ToList();
                }

                return toDisplay;
            }
        } 

        public Bank Bank { get; set; }

        public decimal TotalAmount{
            get { return Checks.Sum(c => c.Amount); }
        }

        public decimal SettledAmount {get { return Checks.Where(c => c.IsSettled).Sum(c => c.Amount); }}

        public decimal RemainingAmount {get { return TotalAmount - SettledAmount; }}
    }
}