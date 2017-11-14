using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;

namespace FBFCheckManagement.Application.Report
{
    public class ReportGenerator
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IBankRepository _bankRepository;

        public ReportGenerator(ICheckRepository checkRepository, IBankRepository bankRepository){
            _checkRepository = checkRepository;
            _bankRepository = bankRepository;
        }

        public DailyReportModel GetDaily(ReportParameter param)
        {
            List<Bank> banks = new List<Bank>();
            List<Check> checks = new List<Check>();

            if (param.ShouldFilterByBank)
            {
                banks.Add(_bankRepository.GetBankById(param.BankId));
                checks = _checkRepository.GetChecksByDateRangeWithBankId(param.Day, param.Day, param.BankId);
            }
            else
            {
                banks = _bankRepository.GetAllBanks();
                checks = _checkRepository.GetChecksByDateRange(param.Day, param.Day);
            }

            DailyReportModel reportModel = new DailyReportModel();
            reportModel.CheckFlag = param.CheckFlag;
            reportModel.Day = param.Day;
            reportModel.DateGenerated = DateTime.Now;

            foreach (var b in banks){
                reportModel.SectionsPerBank.Add(new BankSection(reportModel){
                    Bank = b,
                    Checks = checks.Where(c => c.Bank.Id == b.Id).ToList()
                });
            }

            return reportModel;
        }


        public WeekReportModel GetWeekly(ReportParameter param){
            List<DateTime> daysInWeek = GenerateTheDaysWithinThisRange(param.From, param.To);
            List<DailyReportModel> reports = new List<DailyReportModel>();

            foreach (var day in daysInWeek){
                param.Day = day;
                reports.Add(GetDaily(param));
            }

            WeekReportModel weeklyReport = new WeekReportModel();
            weeklyReport.From = param.From;
            weeklyReport.To = param.To;
            weeklyReport.DailyReports = reports;

            return weeklyReport;
        }

        private List<DateTime> GenerateTheDaysWithinThisRange(DateTime from, DateTime to){
            double numberOfDaysInWeek = (to - from).TotalDays;
            List<DateTime> daysInWeek = new List<DateTime>();
            DateTime currentInterValDay = from;

            daysInWeek.Add(currentInterValDay);
            for (int i = 0; i < numberOfDaysInWeek; i++)
            {
                currentInterValDay = currentInterValDay.AddDays(1);
                daysInWeek.Add(new DateTime(currentInterValDay.Year, currentInterValDay.Month, currentInterValDay.Day));
            }

            return daysInWeek;
        }
    }

    
}