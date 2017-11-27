using System.Linq;
using ClosedXML.Excel;
using FBFCheckManagement.Application.Report;

namespace FBFCheckManagement.WPF.Report
{
    public class ReportExporter
    {
        public void ExporDailytReport(DailyReportModel report, string path){
            XLWorkbook workbookForSaving = new XLWorkbook();
            WorkSheetMaker maker = new WorkSheetMaker(workbookForSaving);
            maker.Make(report);
            
            workbookForSaving.SaveAs(path);
        }

        public void ExportWeeklyReport(WeekReportModel report, string path){
            XLWorkbook workbookForSaving = new XLWorkbook();
            WorkSheetMaker maker = new WorkSheetMaker(workbookForSaving);

            maker.MakeWeeklySummary(report);
            foreach (var daily in report.DailyReports.Where(r => !r.IsTotalForEntireWeek)){
                maker.Make(daily);
            }

            workbookForSaving.SaveAs(path);
        }
    }
}