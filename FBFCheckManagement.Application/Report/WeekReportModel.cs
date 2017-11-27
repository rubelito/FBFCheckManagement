using System;
using System.Collections.Generic;
using System.Linq;

namespace FBFCheckManagement.Application.Report
{
    public class WeekReportModel
    {
        public WeekReportModel(){
            DailyReports = new List<DailyReportModel>();
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string DateRange{
            get{
                return From.ToString("MMMM dd, yyyy") + " - " + To.ToString("MMMM dd, yyyy");
            }
        }

        public List<DailyReportModel> DailyReports { get; set; }

        public decimal Total {get { return DailyReports.Where(d => !d.IsTotalForEntireWeek).Sum(d => d.TotalAmount); }}
        public decimal Settled { get { return DailyReports.Where(d => !d.IsTotalForEntireWeek).Sum(d => d.TotalSettledAmount); } }
        public decimal Remaining { get { return DailyReports.Where(d => !d.IsTotalForEntireWeek).Sum(d => d.TotalRemaining); } }
    }
}