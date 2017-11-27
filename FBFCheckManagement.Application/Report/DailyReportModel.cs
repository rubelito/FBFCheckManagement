using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.DTO;

namespace FBFCheckManagement.Application.Report
{
    public class DailyReportModel
    {
        private readonly WeekReportModel _weeklyReport;

        public DailyReportModel() {
            SectionsPerDepartment = new List<DepartmentSection>();
        }

        public DailyReportModel(WeekReportModel weeklyReport){
            _weeklyReport = weeklyReport;
            SectionsPerDepartment = new List<DepartmentSection>();
        }

        public WeekReportModel WeeklyReport {get { return _weeklyReport; }}
        public bool IsTotalForEntireWeek {get { return _weeklyReport != null; }}

        public DateTime Day { get; set; }

        public string DayOfTheWeek{
            get{
                if (IsTotalForEntireWeek){
                    return "Total";
                }

                return Day.DayOfWeek.ToString();
            }
        }

        public DateTime DateGenerated { get; set; }

        public List<DepartmentSection>  SectionsPerDepartment { get; set; }
        public CheckFlag CheckFlag { get; set; }

        public decimal TotalAmount {get { return SectionsPerDepartment.Sum(s => s.SubTotal); }}

        public decimal TotalSettledAmount { get { return SectionsPerDepartment.Sum(s => s.SettledAmount); }}

        public decimal TotalRemaining { get { return SectionsPerDepartment.Sum(s => s.RemainingAmount); }}
    }
}