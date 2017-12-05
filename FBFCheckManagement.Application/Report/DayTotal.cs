namespace FBFCheckManagement.Application.Report
{
    public class DayTotal : BankSection
    {
        private readonly DepartmentSection _parentDepartment;

        public DayTotal(DepartmentSection parentDepartment)
            : base(parentDepartment){
            _parentDepartment = parentDepartment;
            IsTotalPerDay = true;
        }

        public override decimal Amount{
            get { return _parentDepartment.SubTotal; }
        }

        public override decimal SettledAmount{
            get { return _parentDepartment.SettledAmount; }
        }

        public override decimal RemainingAmount{
            get { return _parentDepartment.RemainingAmount; }
        }
    }
}