using System;
using System.Collections.Generic;
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
        private readonly ICheckRepository _checkRepository;

        private bool _isValidInputs;

        public AddEditCheck(AddEditCheckView model, ICheckRepository checkRepository)
        {
            _model = model;
            _checkRepository = checkRepository;

            InitializeComponent();
        }

        private void AddEditCheck_OnLoaded(object sender, RoutedEventArgs e){
            DataContext = _model;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
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

                IsCanceled = false;
                Close();
            }
        }

        private void ValidateInputs(){
            if (string.IsNullOrEmpty(CheckNumText.Text)){
                MessageBox.Show("Please provide Check #");
                _isValidInputs = false;
            }

            else if (AmountText.Text == "0") {
                MessageBox.Show("Please provide amount");
                _isValidInputs = false;
            }
            else if (string.IsNullOrEmpty(IssuedToTex.Text))
            {
                MessageBox.Show("Please provide issued to");
                _isValidInputs = false;
            }
            else if (!DateIssuedDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please provide issued date");
                _isValidInputs = false;
            }
        }

        private Check MakeCheck(){
            Check c = new Check();
            c.CheckNumber = CheckNumText.Text;
            c.Bank = BankComboBox.SelectedValue as Bank;
            c.Amount = Convert.ToDecimal(AmountText.Text);
            c.IssuedTo = IssuedToTex.Text;
            c.DateIssued = DateIssuedDatePicker.SelectedDate;

            return c;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e){
            IsCanceled = true;
            Close();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (sender as TextBox);
            if (t.Text == "0")
            {
                t.Text = string.Empty;
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (sender as TextBox);
            if (string.IsNullOrEmpty(t.Text))
            {
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
    }
}
