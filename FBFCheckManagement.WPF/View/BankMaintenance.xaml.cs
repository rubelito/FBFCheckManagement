using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;
using Microsoft.VisualBasic;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for BankMaintenance.xaml
    /// </summary>
    public partial class BankMaintenance
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBankRepository _bankRepository;

        public BankMaintenance(IDepartmentRepository departmentRepository, IBankRepository bankRepository){
            _departmentRepository = departmentRepository;
            _bankRepository = bankRepository;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e){
            LoadDepartmentToListView();
        }

        private void LoadDepartmentToListView(){
            DepartmentListBox.ItemsSource = _departmentRepository.GetAllDepartments();
        }

        private void LoadBanksToListView(){
            var dept = DepartmentListBox.SelectedItem as Department;
            if (dept != null){
                BanksListBox.ItemsSource = _bankRepository.GetBanksByDepartment(dept.Id);
            }
        }

        private void DAdd_OnClick(object sender, RoutedEventArgs e){
            var deptName = Interaction.InputBox("Add Another Department Name", "Add Department");
            if (deptName.Length != 0)
            {
                AddNewDepartment(deptName);
                MessageBox.Show(_departmentRepository.StatusMessage);
                LoadDepartmentToListView();
            }
        }

        private void DEdit_OnClick(object sender, RoutedEventArgs e){
            var dept = DepartmentListBox.SelectedItem as Department;
            if (dept == null){
                MessageBox.Show("You must select Department from the List", "Can't Edit", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            var oldDeptName = dept.Name;
            var newDeptName = Interaction.InputBox("Edit Department Name", "Edit Department", oldDeptName);

            if (IsDepartmentExist(newDeptName)){
                MessageBox.Show("Departnment name already exist", "Can't Edit", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                return;
            }

            if (IsOldNameIsNotSimilarToNewName(oldDeptName, newDeptName) &&
                IsNewNameIsNotEmpty(newDeptName)){
                dept.Name = newDeptName;
                _departmentRepository.EditDepartment(dept);

                MessageBox.Show(_departmentRepository.StatusMessage);
                LoadDepartmentToListView();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e){
            var parentDepartment = DepartmentListBox.SelectedItem as Department;
            var bankName = Interaction.InputBox("Add Another Bank Name", "Add Bank");

            if (parentDepartment != null){
                if (bankName.Length != 0){
                    AddNewBank(parentDepartment.Id, bankName);
                    MessageBox.Show(_bankRepository.StatusMessage);
                    LoadBanksToListView();
                }
            }
            else{
                MessageBox.Show("Please select Department.");
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e){
            var bank = BanksListBox.SelectedItem as Bank;
            if (bank == null){
                MessageBox.Show("You must select Bank from the List", "Can't Edit", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }
            var oldBankName = bank.BankName;
            var newBankName = Interaction.InputBox("Edit Bank Name", "Edit Bank", oldBankName);

            if (IsBankExist(newBankName)){
                MessageBox.Show("Bank name already exist", "Can't Edit", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            if (IsOldNameIsNotSimilarToNewName(oldBankName, newBankName) &&
                IsNewNameIsNotEmpty(newBankName)){
                Bank bankToEdit = new Bank(){Id = bank.Id, BankName = newBankName};
                _bankRepository.EditBank(bankToEdit);

                MessageBox.Show(_bankRepository.StatusMessage);
                LoadBanksToListView();
            }
        }

        private bool IsBankExist(string newBankName){
            var banks = BanksListBox.Items.Cast<Bank>();

            return banks.Any(b => b.BankName == newBankName);
        }

        private bool IsDepartmentExist(string newDeptName){
            var depts = DepartmentListBox.Items.Cast<Department>();

            return depts.Any(d => d.Name == newDeptName);
        }

        private bool IsNewNameIsNotEmpty(string newName){
            return !string.IsNullOrWhiteSpace(newName);
        }

        private bool IsOldNameIsNotSimilarToNewName(string oldName, string newName){
            return oldName != newName;
        }

        private void AddNewDepartment(string departmentName){
            var newDept = new Department{Name = departmentName};
            _departmentRepository.AddDepartment(newDept);
        }

        private void AddNewBank(long parentDepartmentId, string bankName){
            var newBank = new Bank{BankName = bankName};
            _bankRepository.AddBank(parentDepartmentId, newBank);
        }

        private void DepartmentListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e){
            LoadBanksToListView();
        }
    }
}
