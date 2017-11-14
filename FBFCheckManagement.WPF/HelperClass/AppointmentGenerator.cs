using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FBFCheckManagement.Application.Domain;
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
                a.IsOnHold = c.HoldDate.HasValue;
                a.IsFunded = c.IsFunded;               
                a.IsSettled = c.IsSettled;
                a.Start = c.HoldDate.HasValue ? c.HoldDate.Value : c.DateIssued.Value;
                a.Subject = c.Bank.BankName + "- " + DecimalAmountToPhp.ConvertToCurrency(c.Amount);
                a.Notes = c.Notes;
                
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
        public bool IsSettled { get; set; }
        public string Notes { get; set; }

        public string ToolTip{
            get{
                if (string.IsNullOrEmpty(Notes))
                    return this.Subject;
                
                    return Notes;               
            }
        }

        public FontStyle FontStyle{
            get{
                if (string.IsNullOrEmpty(Notes))
                    return FontStyles.Normal;

                return FontStyles.Oblique;
            }
        }

        public FontWeight FontWeight{
            get{
                if (string.IsNullOrEmpty(Notes))
                    return FontWeights.Regular;

                return FontWeights.Bold;
            }
        }

        public SolidColorBrush BackColor {
            get{
                SolidColorBrush color;

                if (IsSettled){
                    color = new SolidColorBrush(Colors.LightGreen);
                }
                else if (IsFunded){
                    color = new SolidColorBrush(Colors.Orange);
                }
                else if (IsOnHold){
                    color = new SolidColorBrush(Colors.OrangeRed);
                }
                else{
                    color = new SolidColorBrush(Colors.LightBlue);
                }

                return color;
            }
        }
    }
}