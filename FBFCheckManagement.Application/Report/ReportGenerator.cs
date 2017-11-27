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
        private readonly IDepartmentRepository _deptRepository;
        private readonly IBankRepository _bankRepository;

        private List<Department> _depts;
        private List<Bank> _banks;
        private List<Check> _checks; 

        public ReportGenerator(ICheckRepository checkRepository, IDepartmentRepository deptRepository,
            IBankRepository bankRepository){
            _checkRepository = checkRepository;
            _deptRepository = deptRepository;
            _bankRepository = bankRepository;
        }

        public DailyReportModel GetDaily(ReportParameter param){
            _depts = new List<Department>();
            _banks = new List<Bank>();
            _checks = new List<Check>();

            QueryChecksBasedOnInputParameter(param);
            DailyReportModel reportModel = new DailyReportModel();
            reportModel.CheckFlag = param.CheckFlag;
            reportModel.Day = param.Day;
            reportModel.DateGenerated = DateTime.Now;

            CreateSectionsPerDepartment(reportModel);

            return reportModel;
        }

        private void QueryChecksBasedOnInputParameter(ReportParameter param){
            if (param.ShouldFilterbyDepartment){
                _depts.Add(_deptRepository.GetDepartmentById(param.DepartmentId));
                _banks = _bankRepository.GetBanksByDepartment(param.DepartmentId);
                _checks = _checkRepository.GetChecksByDateRangeWithDepartmentId(param.Day, param.Day, param.DepartmentId);
            }

            else if (param.ShouldFilterByBank){
                Bank bank = _bankRepository.GetBankById(param.BankId);
                _banks.Add(bank);
                _depts.Add(bank.Department);
                _checks = _checkRepository.GetChecksByDateRangeWithBankId(param.Day, param.Day, param.BankId);
            }
            else{
                _depts = _deptRepository.GetAllDepartments();
                _banks = _bankRepository.GetAllBanks();
                _checks = _checkRepository.GetChecksByDateRange(param.Day, param.Day);
            }
        }

        private void CreateSectionsPerDepartment(DailyReportModel reportModel){
                foreach (var currentDept in _depts){
                    var deptSection = new DepartmentSection(reportModel);
                    reportModel.SectionsPerDepartment.Add(deptSection);
                    deptSection.Department = currentDept;
                
                    var banksInDepartment = _banks.Where(b => b.Department.Id == currentDept.Id).ToList();
                    CreateSectionsPerBank(banksInDepartment, deptSection);
                }
            }

                private void CreateSectionsPerBank(List<Bank> banksInDepartment, DepartmentSection deptSection){
                    foreach (var b in banksInDepartment){
                        var bankSection = new BankSection(deptSection);
                        bankSection.Bank = b;
                        bankSection.Checks = _checks.Where(c => c.Bank.Id == b.Id).ToList();

                        deptSection.BankSections.Add(bankSection);
                    }
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

            AttachTotal(weeklyReport);

            return weeklyReport;
        }

        private void AttachTotal(WeekReportModel report){
            DailyReportModel day = report.DailyReports.FirstOrDefault();
            DailyReportModel totalForDay = new DailyReportModel(report);

            foreach (var dept in day.SectionsPerDepartment)
            {
                var totalForDepartment = new DepartmentSection(totalForDay);
                totalForDepartment.Department = dept.Department;
                totalForDay.SectionsPerDepartment.Add(totalForDepartment);

                foreach (var bank in dept.BankSections){
                    var totalForBank = new WeekTotalForBank(totalForDepartment);
                    totalForBank.Bank = bank.Bank;
                    totalForDepartment.BankSections.Add(totalForBank);
                }               
            }
          
            report.DailyReports.Add(totalForDay);
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