using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;

namespace FBFCheckManagement.Application.Report
{
    public class BankSection
    {
        private readonly DepartmentSection _parentDepartment;

        public BankSection(DepartmentSection parentDepartment)
        {
            _parentDepartment = parentDepartment;
            Checks = new List<Check>();
        }

        public List<Check> Checks { get; set; }

        public List<Check> ChecksToDisplay{
            get{
                List<Check> toDisplay = new List<Check>();

                if (_parentDepartment.ParentReport.CheckFlag == CheckFlag.All){
                    toDisplay = Checks;
                }
                if (_parentDepartment.ParentReport.CheckFlag == CheckFlag.NotFunded){
                    toDisplay = Checks.Where(c => !c.IsFunded && !c.IsSettled).ToList();
                }
                if (_parentDepartment.ParentReport.CheckFlag == CheckFlag.Funded){
                    toDisplay = Checks.Where(c => c.IsFunded && !c.IsSettled).ToList();
                }
                if (_parentDepartment.ParentReport.CheckFlag == CheckFlag.Settled){
                    toDisplay = Checks.Where(c => c.IsSettled).ToList();
                }

                return toDisplay;
            }
        }

        public Bank Bank { get; set; }

        public virtual decimal Amount{
            get { return Checks.Sum(c => c.Amount); }
        }

        public virtual decimal SettledAmount {get { return Checks.Where(c => c.IsSettled).Sum(c => c.Amount); }}

        public virtual decimal RemainingAmount {get { return Amount - SettledAmount; }}
    }
}