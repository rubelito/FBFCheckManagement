using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Helper;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Application.Service;
using FBFCheckManagement.WPF.HelperClass;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for MonthChecks.xaml
    /// </summary>
    public partial class MonthChecks : Window
    {
        private readonly ICheckRepository _checkRepository;
        private List<Check> _checksInLastMonth;
        private List<Check> _checksInThisMonth;
        private List<Check> _checksInNextMonth;
        private List<Check> _allChecks; 


        private WeekRange _firstWeek;
        private WeekRange _secondWeek;
        private WeekRange _thirdWeek;
        private WeekRange _fourthWeek;
        private WeekRange _fifthWeek;

        public MonthChecks(ICheckRepository checkRepository){
            _checkRepository = checkRepository;           
            InitializeComponent();
        }

        private void MonthChecks_OnLoaded(object sender, RoutedEventArgs e){

            ScheduleView.AppointmentDeleting += ScheduleViewOnAppointmentDeleting;
            ScheduleView.AppointmentCreating += ScheduleViewOnAppointmentCreating;
            ScheduleView.AppointmentEditing += ScheduleViewOnAppointmentEditing;
            LoadChecks(ScheduleView.CurrentDate);
        }

        private void LoadChecks(DateTime currentDate){

            AppointmentGenerator generator = new AppointmentGenerator();

            DateTime lastMonth = currentDate.AddMonths(-1);
            DateTime nextMonth = currentDate.AddMonths(1);

            _checksInLastMonth = _checkRepository.GetChecksByMonth(lastMonth.Year, lastMonth.Month);
            _checksInThisMonth = _checkRepository.GetChecksByMonth(currentDate.Year, currentDate.Month);
            _checksInNextMonth = _checkRepository.GetChecksByMonth(nextMonth.Year, nextMonth.Month);

            _allChecks = new List<Check>();
            _allChecks.AddRange(_checksInLastMonth);
            _allChecks.AddRange(_checksInThisMonth);
            _allChecks.AddRange(_checksInNextMonth);

            List<AppointmentCheck> allAppointments = generator.CreateAppointmentObjects(_allChecks);

            ScheduleView.AppointmentsSource = allAppointments;
            ScheduleView.Commit();
        }

        private void ScheduleView_VisibleRangeChanged(object sender, EventArgs e){
            LoadChecks(ScheduleView.CurrentDate);
            HighlightNextAndPreviousMonth(ScheduleView.CurrentDate);
            SetRangeOfWeeks();
            ComputeChecksAmountsPerWeek();
        }

        private void ScheduleViewOnAppointmentEditing(object sender, AppointmentEditingEventArgs e){

            AppointmentCheck selectedCheck = (AppointmentCheck)e.Appointment;
            Check check = _checkRepository.GetCheckById(selectedCheck.Id);


            ViewChecks vc = new ViewChecks(check, _checkRepository);
            vc.ShowDialog();

            if (vc.IsCancelled == false){
                LoadChecks(ScheduleView.CurrentDate);
                HighlightNextAndPreviousMonth(ScheduleView.CurrentDate);
                SetRangeOfWeeks();
                ComputeChecksAmountsPerWeek();
            }
        }

        private void ScheduleView_OnShowDialog(object sender, ShowDialogEventArgs e){
            e.Cancel = true;
        }

        private void ScheduleViewOnAppointmentCreating(object sender, AppointmentCreatingEventArgs e){
            
            e.Cancel = true;
        }

        private void ScheduleViewOnAppointmentDeleting(object sender, AppointmentDeletingEventArgs e){
            e.Cancel = true;
        }

        private void HighlightNextAndPreviousMonth(DateTime dateNow)
        {
            var previousMonth = dateNow.AddMonths(-1);
            var previousMonthSlot = new List<Slot>(DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            for (int x = 1; x <= previousMonthSlot.Capacity; x++)
            {
                previousMonthSlot.Add(new Slot(new DateTime(previousMonth.Year, previousMonth.Month, x, 0, 1, 1),
                    new DateTime(previousMonth.Year, previousMonth.Month, x, 1, 1, 1)));
            }
            var nextMonth = dateNow.AddMonths(1);
            var nextMonthSlot = new List<Slot>(DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            for (int x = 1; x <= nextMonthSlot.Capacity; x++)
            {
                nextMonthSlot.Add(new Slot(new DateTime(nextMonth.Year, nextMonth.Month, x, 0, 1, 1),
                    new DateTime(nextMonth.Year, nextMonth.Month, x, 1, 1, 1)));
            }
            previousMonthSlot.AddRange(nextMonthSlot);
            ScheduleView.SpecialSlotsSource = new ObservableCollection<Slot>(previousMonthSlot);
        }

        private void ComputeChecksAmountsPerWeek(){

            if (IsControlReady()){
                CheckService service = new CheckService(_checkRepository);

                Week1.Text =
                    DecimalAmountToPhp.ConvertToPhp(service.ComputeChecksTotalInTheWeek(_firstWeek.Start,
                        _allChecks));

                Week2.Text =
                    DecimalAmountToPhp.ConvertToPhp(service.ComputeChecksTotalInTheWeek(_secondWeek.Start,
                        _allChecks));

                Week3.Text =
                    DecimalAmountToPhp.ConvertToPhp(service.ComputeChecksTotalInTheWeek(_thirdWeek.Start,
                        _allChecks));

                Week4.Text =
                    DecimalAmountToPhp.ConvertToPhp(service.ComputeChecksTotalInTheWeek(_fourthWeek.Start,
                        _allChecks));

                Week5.Text =
                    DecimalAmountToPhp.ConvertToPhp(service.ComputeChecksTotalInTheWeek(_fifthWeek.Start,
                        _allChecks));
            }
        }

        private bool IsControlReady(){
            return Week1 != null;
        }

        private void MonthChecks_OnContentRendered(object sender, EventArgs e){
            SetRangeOfWeeks();
            ComputeChecksAmountsPerWeek();
        }

        private void SetRangeOfWeeks(){
           IDateSpan range = ScheduleView.VisibleRange;

            _firstWeek = CreateSevenDaysWeek(range.Start, true);
            _secondWeek = CreateSevenDaysWeek(_firstWeek.End, false);
            _thirdWeek = CreateSevenDaysWeek(_secondWeek.End, false);
            _fourthWeek = CreateSevenDaysWeek(_thirdWeek.End, false);
            _fifthWeek = CreateSevenDaysWeek(_fourthWeek.End, false);
        }

        private WeekRange CreateSevenDaysWeek(DateTime start, bool isFirstWeek){
            WeekRange w = new WeekRange();

            if (isFirstWeek){
                w.Start = start;
            }
            else{
                w.Start = start.AddDays(1);
            }

            w.End = w.Start.AddDays(6);

            return w;
        }
    }
}
