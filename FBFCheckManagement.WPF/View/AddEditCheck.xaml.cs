using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.WPF.ViewModel;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for AddEditCheck.xaml
    /// </summary>
    public partial class AddEditCheck : Window
    {
        private readonly AddEditCheckView _model;
        private readonly IDepartmentRepository _deptRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ICheckRepository _checkRepository;

        private List<Department> _departments;
        private List<Bank> _banks; 

        private bool _isValidInputs;

        public AddEditCheck(AddEditCheckView model, IDepartmentRepository deptRepository, IBankRepository bankRepository, 
            ICheckRepository checkRepository){
            _model = model;
            _deptRepository = deptRepository;
            _bankRepository = bankRepository;
            _checkRepository = checkRepository;

            InitializeComponent();
        }

        private void AddEditCheck_OnLoaded(object sender, RoutedEventArgs e){
            LoadDepartments();
            SetDepartmentToCurrent();
            DataContext = _model;
        }

        private void LoadDepartments(){
            _departments = _deptRepository.GetAllDepartments();
            _model.Departments = CollectionViewSource.GetDefaultView(
                new ObservableCollection<Department>(_departments));
            
        }

        private void SetDepartmentToCurrent(){
            if (_model.Operation == Operation.Add){
                _model.SelectedDepartment = _departments[0];
            }
            else if (_model.Operation == Operation.Edit){
                var currentDept = _model.Check.Bank.Department;
                _model.SelectedDepartment = _departments.FirstOrDefault(d => d.Id == currentDept.Id);
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e){
            _isValidInputs = true;
            ValidateInputs();
            Check check = MakeCheck();

            if (_isValidInputs){
                if (_model.Operation == Operation.Add){
                    check.CreatedDate = DateTime.Now;
                    _checkRepository.Add(check);
                }
                else if (_model.Operation == Operation.Edit){
                    check.ModifiedDate = DateTime.Now;
                    check.Id = _model.CheckToEdit;
                    _checkRepository.Update(check);
                }

                if (_checkRepository.IsSuccess){
                    IsCanceled = false;
                    Close();
                }
                else{
                    MessageBox.Show(_checkRepository.ErrorMessage);
                }             
            }
        }

        private void ValidateInputs(){
            if (string.IsNullOrEmpty(CheckNumText.Text)){
                MessageBox.Show("Please provide Check #");
                _isValidInputs = false;
            }

            else if (DepartmentComboBox.SelectedValue == null){
                MessageBox.Show("Please select Department");
                _isValidInputs = false;
            }

            else if (BankComboBox.SelectedValue == null){
                MessageBox.Show("Please select Bank");
                _isValidInputs = false;
            }

            else if (AmountText.Text == "0"){
                MessageBox.Show("Please provide amount");
                _isValidInputs = false;
            }
            else if (string.IsNullOrEmpty(IssuedToTex.Text)){
                MessageBox.Show("Please provide issued to");
                _isValidInputs = false;
            }
            else if (!DateIssuedDatePicker.SelectedDate.HasValue){
                MessageBox.Show("Please provide issued date");
                _isValidInputs = false;
            }
        }

        private Check MakeCheck(){
            Check c = new Check();
            c.CheckNumber = CheckNumText.Text;
            c.Bank = _model.SelectedBank;
            c.Amount = Convert.ToDecimal(AmountText.Text);
            c.IssuedTo = IssuedToTex.Text;
            c.DateIssued = DateIssuedDatePicker.SelectedDate;

            return c;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e){
            IsCanceled = true;
            Close();
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

        public bool IsCanceled { get; set; }

        private void AddEditCheck_OnContentRendered(object sender, EventArgs e){
            if (_model.Operation == Operation.Add){
                BankComboBox.SelectedIndex = 0;
            }
            else if (_model.Operation == Operation.Edit){
                SetComboBoxCurrentToCurrentBank();
            }
        }

        private void SetComboBoxCurrentToCurrentBank(){
            int index = 0;
            foreach (var item in BankComboBox.Items){
                Bank b = item as Bank;
                if (b.Id == _model.Check.Bank.Id){
                    break;
                }
                index++;
            }

            BankComboBox.SelectedIndex = index;
        }

        private void DepartmentComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            LoadBanks();
        }

        private void LoadBanks(){
            var dept = _model.SelectedDepartment;

            _banks = _bankRepository.GetBanksByDepartment(dept.Id);
            _model.Banks = CollectionViewSource.GetDefaultView(
                    new ObservableCollection<Bank>(_banks));
            if (_model.Operation == Operation.Add){
                _model.SelectedBank = _banks[0];
            }
        }
    }
}
