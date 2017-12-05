using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Helper;
using FBFCheckManagement.Application.Report;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Application.Service;
using FBFCheckManagement.WPF.HelperClass;
using FBFCheckManagement.WPF.Report;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for MonthChecks.xaml
    /// </summary>
    public partial class MonthChecks
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IDepartmentRepository _deptRepository;
        private readonly IBankRepository _bankRepository;

        private List<Check> _checksInLastMonth;
        private List<Check> _checksInThisMonth;
        private List<Check> _checksInNextMonth;
        private List<Check> _allChecks;

        private WeekRange _firstWeek;
        private WeekRange _secondWeek;
        private WeekRange _thirdWeek;
        private WeekRange _fourthWeek;
        private WeekRange _fifthWeek;

        private bool _isUserChangeComboBox;

        private ScheduleViewModel _model;

        public MonthChecks(ICheckRepository checkRepository, IDepartmentRepository deptRepository,
            IBankRepository bankRepository){
            _checkRepository = checkRepository;
            _deptRepository = deptRepository;
            _bankRepository = bankRepository;
            InitializeComponent();
            _model = new ScheduleViewModel();
        }

        private void MonthChecks_OnLoaded(object sender, RoutedEventArgs e){
            ScheduleView.AppointmentDeleting += ScheduleViewOnAppointmentDeleting;
            ScheduleView.AppointmentCreating += ScheduleViewOnAppointmentCreating;
            ScheduleView.AppointmentEditing += ScheduleViewOnAppointmentEditing;
        }

        private void MonthChecks_OnContentRendered(object sender, EventArgs e){
            LoadDepartments();
            SetRangeOfWeeks();
            LoadChecks(ScheduleView.CurrentDate);
            HighlightNextAndPreviousMonth(ScheduleView.CurrentDate);
            ComputeChecksAmountsPerWeek();
            DataContext = _model;
        }

        private void LoadDepartments(){
            List<Department> depts = _deptRepository.GetAllDepartments();
            depts.Insert(0, new Department(){Id = 0, Name = "All"});
            DepartmentComboBox.ItemsSource = depts;
        }

        private void LoadChecks(DateTime currentDate){
            AppointmentGenerator generator = new AppointmentGenerator();

            DateTime lastMonth = currentDate.AddMonths(-1);
            DateTime nextMonth = currentDate.AddMonths(1);

            YearMonthInfo forLastMonth = CreateSearchParameter(lastMonth);
            YearMonthInfo forCurrentMonth = CreateSearchParameter(currentDate);
            YearMonthInfo forNextMonth = CreateSearchParameter(nextMonth);

            _checksInLastMonth = _checkRepository.GetChecksByMonth(forLastMonth);
            _checksInThisMonth = _checkRepository.GetChecksByMonth(forCurrentMonth);
            _checksInNextMonth = _checkRepository.GetChecksByMonth(forNextMonth);

            _allChecks = new List<Check>();
            _allChecks.AddRange(_checksInLastMonth);
            _allChecks.AddRange(_checksInThisMonth);
            _allChecks.AddRange(_checksInNextMonth);

            List<Check> checksWithFlag = ApplyCheckFlagQuery();

            List<AppointmentCheck> allAppointments = generator.CreateAppointmentObjects(checksWithFlag);

            ScheduleView.AppointmentsSource = allAppointments;
            ScheduleView.Commit();
            ScheduleView.SelectedAppointment = null;
        }

        private List<Check> ApplyCheckFlagQuery(){
            List<Check> preQuery = _allChecks;

            CheckFlag status = (CheckFlag) CheckStatusComboBox.SelectedItem;

            if (status == CheckFlag.Funded){
                preQuery = _allChecks.Where(c => c.IsFunded && !c.IsSettled).ToList();
            }
            if (status == CheckFlag.NotFunded){
                preQuery = _allChecks.Where(c => !c.IsFunded && !c.IsSettled).ToList();
            }
            if (status == CheckFlag.Settled){
                preQuery = _allChecks.Where(c => c.IsSettled).ToList();
            }

            return preQuery;
        }

        private YearMonthInfo CreateSearchParameter(DateTime currentDate){
            YearMonthInfo i = new YearMonthInfo();
            i.Year = currentDate.Year;
            i.Month = currentDate.Month;
            i.ShouldFilterByStatus = false;

            Bank selectedBank = BankComboBox.SelectedValue as Bank;
            Department selectedDept = DepartmentComboBox.SelectedValue as Department;

            if (selectedDept.Name == "All"){
                i.ShouldFilterByDepartment = false;
            }
            else{
                i.ShouldFilterByDepartment = true;
                i.DepartmentId = selectedDept.Id;
            }

            if (selectedBank == null || selectedBank.BankName == "All"){
                i.ShouldFilterByBank = false;
            }
            else{
                i.ShouldFilterByBank = true;
                i.BankId = selectedBank.Id;
            }

            return i;
        }

        private void ScheduleView_VisibleRangeChanged(object sender, EventArgs e){
            if (IsControlReady()){
                LoadChecks(ScheduleView.CurrentDate);
                HighlightNextAndPreviousMonth(ScheduleView.CurrentDate);
                SetRangeOfWeeks();
                ComputeChecksAmountsPerWeek();
            }
        }

        private void ScheduleViewOnAppointmentEditing(object sender, AppointmentEditingEventArgs e){
            AppointmentCheck selectedCheck = (AppointmentCheck) e.Appointment;
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

        private void HighlightNextAndPreviousMonth(DateTime dateNow){
            var previousMonth = dateNow.AddMonths(-1);
            var previousMonthSlot = new List<Slot>(DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            for (int x = 1; x <= previousMonthSlot.Capacity; x++){
                previousMonthSlot.Add(new Slot(new DateTime(previousMonth.Year, previousMonth.Month, x, 0, 1, 1),
                    new DateTime(previousMonth.Year, previousMonth.Month, x, 1, 1, 1)));
            }
            var nextMonth = dateNow.AddMonths(1);
            var nextMonthSlot = new List<Slot>(DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            for (int x = 1; x <= nextMonthSlot.Capacity; x++){
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
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeChecksTotalInTheWeek(_firstWeek.Start,
                        _allChecks));

                Week2.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeChecksTotalInTheWeek(_secondWeek.Start,
                        _allChecks));

                Week3.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeChecksTotalInTheWeek(_thirdWeek.Start,
                        _allChecks));

                Week4.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeChecksTotalInTheWeek(_fourthWeek.Start,
                        _allChecks));

                Week5.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeChecksTotalInTheWeek(_fifthWeek.Start,
                        _allChecks));

                // This Section if for subtracting remaining

                WeekRemaining1.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeRemainingAmountInTheWeek(_firstWeek.Start,
                        _allChecks));

                WeekRemaining2.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeRemainingAmountInTheWeek(_secondWeek.Start,
                        _allChecks));

                WeekRemaining3.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeRemainingAmountInTheWeek(_thirdWeek.Start,
                        _allChecks));

                WeekRemaining4.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeRemainingAmountInTheWeek(_fourthWeek.Start,
                        _allChecks));

                WeekRemaining5.Text =
                    DecimalAmountToPhp.ConvertToCurrency(service.ComputeRemainingAmountInTheWeek(_fifthWeek.Start,
                        _allChecks));

            }
        }

        private bool IsControlReady(){
            return Week1 != null;
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

        private void BankComboBox_OnDropDownOpened(object sender, EventArgs e){
            _isUserChangeComboBox = true;
        }

        private void CheckStatusComboBox_OnDropDownOpened(object sender, EventArgs e){
            _isUserChangeComboBox = true;
        }

        private void BankComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            QueryDataWhenSelectionChange();
        }

        private void CheckStatusComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            QueryDataWhenSelectionChange();
        }

        private void QueryDataWhenSelectionChange(){
            if (_isUserChangeComboBox){
                LoadChecks(ScheduleView.CurrentDate);
                HighlightNextAndPreviousMonth(ScheduleView.CurrentDate);
                SetRangeOfWeeks();
                ComputeChecksAmountsPerWeek();
            }
        }

        private void PrintDailyButton_OnClick(object sender, RoutedEventArgs e){
            MakeReport(ReportType.Daily);
        }

        private void WeeklyDailyButton_OnClick(object sender, RoutedEventArgs e){
            MakeReport(ReportType.Weekly);
        }

        private void MakeReport(ReportType type){
            DateTime day = ScheduleView.SelectedSlot.Start;
            DateTime from = day.GetFirstDayOfWeek();
            DateTime to = day.GetLastDayOfWeek();

            CheckFlag selectedFlag = (CheckFlag) CheckStatusComboBox.SelectedItem;
            Department selectedDepartment = (DepartmentComboBox.SelectedItem as Department);
            Bank selectedBank = (BankComboBox.SelectedItem as Bank);

            ReportParameter param = new ReportParameter(){
                Type = type,
                CheckFlag = selectedFlag,
                DepartmentId = selectedDepartment.Id,
                BankId = (int) selectedBank.Id,
                DepartmentName = selectedDepartment.Name,
                BankName = selectedBank.BankName,
                Day = day,
                From = from,
                To = to
            };

            FileNameMaker maker = new FileNameMaker(param);
            string fileName = maker.GetFileName();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = fileName;
            if (saveFileDialog.ShowDialog() == true){
                try{
                    ExportReport(param, saveFileDialog.FileName);
                    MessageBox.Show("Report Generated!");
                }
                catch (Exception ex){
                    MessageBox.Show(ex.Message, "Error Exporting Report");
                }
            }
        }

        private void ExportReport(ReportParameter param, string fileName){
            ReportExporter exporter = new ReportExporter();
            ReportGenerator gen = new ReportGenerator(_checkRepository, _deptRepository, _bankRepository);

            if (param.Type == ReportType.Daily){
                var dailyReportModel = gen.GetDaily(param);
                exporter.ExporDailytReport(dailyReportModel, fileName);
            }
            if (param.Type == ReportType.Weekly){
                var weeklyReportModel = gen.GetWeekly(param);
                exporter.ExportWeeklyReport(weeklyReportModel, fileName);
            }
        }

        private void DepartmentComboBox_OnDropDownOpened(object sender, EventArgs e){
            _isUserChangeComboBox = true;
        }

        private void DepartmentComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            LoadBanks();
            QueryDataWhenSelectionChange();
        }

        private void LoadBanks(){
            var dept = DepartmentComboBox.SelectedItem as Department;
            List<Bank> banks = new List<Bank>();
            banks.Insert(0, new Bank() { Id = 0, BankName = "All" });

            if (dept != null && dept.Id != 0){
                BankComboBox.IsEnabled = true;
                banks.AddRange(_bankRepository.GetBanksByDepartment(dept.Id));
                BankComboBox.ItemsSource = banks;
            }
            else{
                BankComboBox.ItemsSource = banks;
                BankComboBox.IsEnabled = false;
            }
        }
    }

    public class ScheduleViewModel : BindableBase
    {
        private Slot _selectedDaySlot;

        public Slot SelectedDaySlot{
            get { return _selectedDaySlot; }
            set { if (value != null) SetProperty(ref _selectedDaySlot, value); }
        }
    }
}
