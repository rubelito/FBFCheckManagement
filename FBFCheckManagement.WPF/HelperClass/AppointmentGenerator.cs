using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Application.Service;
using Telerik.Windows.Controls.ScheduleView;

namespace FBFCheckManagement.WPF.HelperClass
{
    public class AppointmentGenerator
    {
        public List<AppointmentCheck> CreateAppointmentObjects(List<Check> checks)
        {
            List<AppointmentCheck> appointments = new List<AppointmentCheck>();

            foreach (var c in checks){
                AppointmentCheck a = new AppointmentCheck();

                a.Id = c.Id;
                a.IsFunded = c.IsFunded;
                a.IsOnHold = c.HoldDate.HasValue;
                a.Start = c.HoldDate.HasValue ? c.HoldDate.Value : c.DateIssued.Value;
                a.Subject = DecimalAmountToPhp.ConvertToPhp(c.Amount);

                a.End = a.Start.AddHours(1);

                appointments.Add(a);
            }

            return appointments;
        }
    }

    public class AppointmentCheck : Appointment
    {
        public long Id { get; set; }
        public bool IsFunded { get; set; }
        public bool IsOnHold { get; set; }

        public SolidColorBrush BackColor {
            get{
                SolidColorBrush color = null;

                if (IsFunded){
                    color = new SolidColorBrush(Colors.LightGreen);
                }
                else if (IsOnHold){
                    color = new SolidColorBrush(Colors.Pink);
                }
                else{
                    color = new SolidColorBrush(Colors.LightBlue);
                }

                return color;
            }
        }
    }
}