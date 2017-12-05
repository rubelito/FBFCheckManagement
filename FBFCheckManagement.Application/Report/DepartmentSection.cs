using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.Report
{
    public class DepartmentSection
    {
        private DailyReportModel _parentReport;

        public DepartmentSection(DailyReportModel parentReport)
        {
            _parentReport = parentReport;
            BankSections = new List<BankSection>();
        }

        public DailyReportModel ParentReport
        {
            get { return _parentReport; }
        }

        public List<BankSection> BankSections { get; set; }

        private List<BankSection> BanksToCompute
        {
            get { return BankSections.Where(b => !b.IsTotalPerDay).ToList(); }
        }

        public Department Department { get; set; }

        public decimal SubTotal
        {
            get { return BanksToCompute.Sum(b => b.Amount); }
        }

        public decimal SettledAmount
        {
            get { return BanksToCompute.Sum(b => b.SettledAmount); }
        }

        public decimal RemainingAmount
        {
            get { return BanksToCompute.Sum(b => b.RemainingAmount); }
        }
    }
}
