using System;
using System.Globalization;

namespace FBFCheckManagement.Application.Helper
{
    public static class DateHelper
    {
        public static DateTime GetFirstDayOfWeek(this DateTime date){
            DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            int offset = fdow - date.DayOfWeek;
            DateTime fdowDate = date.AddDays(offset);
            return fdowDate;
        }

        public static DateTime GetLastDayOfWeek(this DateTime date){
         
            DateTime ldowDate = date.GetFirstDayOfWeek().AddDays(6);
            return ldowDate;
        }
    }
}