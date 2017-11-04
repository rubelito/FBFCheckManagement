using System.Windows;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;
using Microsoft.VisualBasic;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for BankMaintenance.xaml
    /// </summary>
    public partial class BankMaintenance : Window
    {
        private readonly IBankRepository _bankRepository;

        public BankMaintenance(IBankRepository bankRepository){
            _bankRepository = bankRepository;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e){
            LoadBanksToListView();
        }

        private void LoadBanksToListView(){
            BanksListBox.ItemsSource = _bankRepository.GetAllBanks();
        }

        private void add_Click(object sender, RoutedEventArgs e){
           
            var bankName = Interaction.InputBox("Add Another Bank Name", "Add Bank");
            if (bankName.Length != 0)
            {
               AddNewBank(bankName);
               MessageBox.Show(_bankRepository.StatusMessage);
                LoadBanksToListView();
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
            if (IsOldBankNameIsNotSimilarWithNewName(oldBankName, newBankName) &&
                IsNewBankNameIsnotEmpty(newBankName)){
                bank.BankName = newBankName;
                _bankRepository.EditBank(bank);

                MessageBox.Show(_bankRepository.StatusMessage);
                LoadBanksToListView();
            }
        }

        private bool IsNewBankNameIsnotEmpty(string newBankName)
        {
            return !string.IsNullOrWhiteSpace(newBankName);
        }

        private bool IsOldBankNameIsNotSimilarWithNewName(string oldBankName, string newBankName)
        {
            return oldBankName != newBankName;
        }

        private void AddNewBank(string bankName){
            var newBank = new Bank { BankName = bankName };
            _bankRepository.AddBank(newBank);
        }
    }
}
