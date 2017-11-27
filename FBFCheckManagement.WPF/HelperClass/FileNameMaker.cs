using System;
using FBFCheckManagement.Application.Report;

namespace FBFCheckManagement.WPF.HelperClass
{
    public class FileNameMaker
    {
        private readonly ReportParameter _param;

        public FileNameMaker(ReportParameter param){
            _param = param;
        }

        public string GetFileName(){
            string checkFlag = Enum.GetName(_param.CheckFlag.GetType(), _param.CheckFlag);
            string departmentName = _param.DepartmentName;
            string bankName = _param.BankName;
            string typeName = _param.Type == ReportType.Daily ? "Daily" : "Weekly";

            string dateInFileName = _param.Type == ReportType.Daily
                ? _param.Day.ToString("MMM dd, yyyy")
                : _param.From.ToString("MMM dd, yyyy") + " - " + _param.To.ToString("MMM dd, yyyy");

             return typeName + " " + departmentName + " " + bankName + " " + dateInFileName + " (" + checkFlag + ")";
        }
    }
}