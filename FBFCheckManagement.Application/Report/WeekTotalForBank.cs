using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.Report
{
    public class WeekTotalForBank : BankSection
    {
        private readonly DepartmentSection _parentDepartment;

        public WeekTotalForBank(DepartmentSection parentDepartment)
            : base(parentDepartment)
        {
            _parentDepartment = parentDepartment;
            Checks = new List<Check>();
            IsTotalPerDay = false;
        }

        private WeekReportModel GetParentWeeklyReport()
        {
            return _parentDepartment.ParentReport.WeeklyReport;
        }

        private List<BankSection> BanksInEntireWeek()
        {
            List<BankSection> banksInEntireWeek = new List<BankSection>();
            WeekReportModel parent = GetParentWeeklyReport();

            foreach (var day in parent.DailyReports.Where(d => !d.IsTotalForEntireWeek).ToList())
            {
                foreach (var dept in day.SectionsPerDepartment)
                {
                    List<BankSection> banksInDepartment = dept.BankSections.Where(b => b.Bank.Id == Bank.Id).ToList();
                    banksInEntireWeek.AddRange(banksInDepartment);
                }
            }

            return banksInEntireWeek;
        }

        public override decimal Amount
        {
            get { return BanksInEntireWeek().Sum(b => b.Amount); }
        }

        public override decimal SettledAmount
        {
            get { return BanksInEntireWeek().Sum(b => b.SettledAmount); }
        }

        public override decimal RemainingAmount
        {
            get { return BanksInEntireWeek().Sum(b => b.RemainingAmount); }
        }
    }
}