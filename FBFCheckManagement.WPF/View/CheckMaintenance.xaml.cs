using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Application.Service;
using FBFCheckManagement.WPF.ViewModel;
using Telerik.Windows.Controls;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for CheckMaintenance.xaml
    /// </summary>
    public partial class CheckMaintenance
    {
        private CheckMaintenanceView _c;
        private readonly IDepartmentRepository _deptRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ICheckRepository _checkRepository;
        private List<Department> _departments;
        private List<Bank> _banks;

        public CheckMaintenance(IDepartmentRepository deptRepository, IBankRepository bankRepository,
            ICheckRepository checkRepository){
            _deptRepository = deptRepository;
            _bankRepository = bankRepository;
            _checkRepository = checkRepository;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e){
            _c = new CheckMaintenanceView();

            DataPager.PageIndexChanged += DataPagerOnPageIndexChanged;

            DataContext = _c;
            LoadDepartment();
            SetCurrentItemForDepartment();
            SetDefaultValuesOfModel();

            DisplaySearch();
        }

        private void DisplaySearch(){
            var result = Search(0, DataPager.PageSize);

            _c.PagedChecks = CollectionViewSource.GetDefaultView(
                new ObservableCollection<Check>(result.Results));
            _c.PagedChecks.Refresh();

            DataPager.ItemCount = result.TotalItems;
            DataPager.MoveToFirstPage();
        }

        private void LoadDepartment(){
            try{
                _departments = _deptRepository.GetAllDepartments();
                _departments.Insert(0, new Department(){Id = 0, Name = "All"});
                _c.Departments = CollectionViewSource.GetDefaultView(
                    new ObservableCollection<Department>(_departments));
                _c.HasSelectedDepartment = false;
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        private void SetCurrentItemForDepartment(){
            _c.SelectedDepartment = _departments[0];
        }

        private CheckPagingResult Search(int currentPage, int pageSize){
            CheckService service = new CheckService(_checkRepository);

            CheckPagingRequest r = new CheckPagingRequest();
            SearchCriteria s = BuilSearchQuery();

            r.PageSize = pageSize;
            r.CurrentPage = currentPage;
            r.SearchCriteria = s;

            CheckPagingResult result = service.SearchWithPagination(r);
            return result;
        }

        private SearchCriteria BuilSearchQuery(){
            SearchCriteria s = new SearchCriteria();

            s.CheckNumber = _c.CheckNumber;
            s.SelectedDepartment = _c.SelectedDepartment.Name != "All" ? _c.SelectedDepartment : null;
            s.SelectedBank =  (_c.SelectedBank == null ||_c.SelectedBank.BankName != "All") ? _c.SelectedBank : null;
            s.AmountFrom = _c.AmountFrom;
            s.AmountTo = _c.AmountTo;
            s.IssuedDateFrom = _c.IssuedDateFrom;
            s.IssuedDateTo = _c.IssuedDateTo;
            s.IssuedTo = _c.IssuedTo;
            s.CreatedDateFrom = _c.CreatedDateFrom;
            s.CreatedDateTo = _c.CreatedDateTo;
            s.Order = _c.SelectedOrder;
            s.OrderBy = _c.SelectedOrderBy;

            return s;
        }

        private void DataPagerOnPageIndexChanged(object sender, PageIndexChangedEventArgs e){
            CheckPagingResult result = Search(e.NewPageIndex, DataPager.PageSize);

            var filteredItem = CollectionViewSource.GetDefaultView(
                new ObservableCollection<Check>(result.Results));
            DataPager.ItemCount = result.TotalItems;

            _c.PagedChecks = filteredItem;
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e){
            DisplaySearch();
        }

        private void ClearSearchButton_OnClick(object sender, RoutedEventArgs e){
            SetDefaultValuesOfModel();
            DisplaySearch();
        }

        private void SetDefaultValuesOfModel(){
            _c.CheckNumber = string.Empty;
            _c.Departments.MoveCurrentToFirst();
            _c.AmountFrom = 0;
            _c.AmountTo = 0;
            _c.IssuedDateFrom = null;
            _c.IssuedDateTo = null;
            _c.IssuedTo = string.Empty;
            _c.CreatedDateFrom = null;
            _c.CreatedDateTo = null;
            OrderByComboBox.SelectedIndex = 0;
            _c.SelectedOrder = OrderByArrangement.Descending;
            _c.SetOrderby = new ComboBoxItem(){Content = "Created Date"};
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e){
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e){
            TextBox t = (sender as TextBox);
            if (t.Text == "0"){
                t.Text = string.Empty;
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e){
            TextBox t = (sender as TextBox);
            if (string.IsNullOrEmpty(t.Text)){
                t.Text = "0";
            }
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e){
            AddEditCheckView addModel = new AddEditCheckView();
            addModel.Check = new Check();
            addModel.Operation = Operation.Add;
            AddEditCheck addWindow = new AddEditCheck(addModel, _deptRepository, _bankRepository, _checkRepository);

            addWindow.ShowDialog();

            if (addWindow.IsCanceled == false){
                DisplaySearch();
            }
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e){
            AddEditCheckView editModel = new AddEditCheckView();
            editModel.CheckToEdit = _c.SelectedCheck.Id;
            editModel.Check = _c.SelectedCheck;
            editModel.Operation = Operation.Edit;
            AddEditCheck addWindow = new AddEditCheck(editModel, _deptRepository, _bankRepository, _checkRepository);

            addWindow.ShowDialog();

            if (addWindow.IsCanceled == false){
                DisplaySearch();
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e){
            var yesorno = MessageBox.Show("Are you sure do you want to Delete this record?", "Delete Record",
                MessageBoxButton.YesNo);
            if (yesorno == MessageBoxResult.Yes){
                Check checkToRemove = _c.SelectedCheck;
                _checkRepository.Delete(checkToRemove.Id);

                DisplaySearch();
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            LoadBanks();
        }

        private void LoadBanks(){
            var dept = _c.Departments.CurrentItem as Department;

            if (dept != null && dept.Id != 0){
                _banks = _bankRepository.GetBanksByDepartment(dept.Id);
                _banks.Insert(0, new Bank(){Id = 0, BankName = "All"});
                _c.Banks = CollectionViewSource.GetDefaultView(
                    new ObservableCollection<Bank>(_banks));
                SetCurrentItemForBanks();
                _c.HasSelectedDepartment = true;
            }
            else{
                _banks = new List<Bank>();
                _c.Banks = null;
                _c.HasSelectedDepartment = false;
            }
        }

        private void SetCurrentItemForBanks(){
            _c.SelectedBank = _banks[0];
        }
    }
}
