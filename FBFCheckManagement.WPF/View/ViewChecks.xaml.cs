using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for ViewChecks.xaml
    /// </summary>
    public partial class ViewChecks
    {
        private readonly Check _check;
        private readonly ICheckRepository _checkRepository;

        public bool IsCancelled { get; set; }

        public ViewChecks(Check checks, ICheckRepository checkRepository){
            _check = checks;
            _checkRepository = checkRepository;
            InitializeComponent();
        }

        private void ViewChecks_OnContentRendered(object sender, EventArgs e){
            DataContext = _check;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e){
            IsCancelled = true;
            Close();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e){

            DateTime? onHoldDate = GetOnHoldDate();

            bool isFunded = IsFunded.IsChecked.HasValue && IsFunded.IsChecked.Value;
            bool isSettled = IsSettled.IsChecked.HasValue && IsSettled.IsChecked.Value;

            _check.HoldDate = onHoldDate;
            _check.IsFunded = isFunded;
            _check.IsSettled = isSettled;
            _check.Notes = NotesText.Text;

            _checkRepository.Update(_check);

            IsCancelled = false;
            Close();
        }

        private DateTime? GetOnHoldDate(){
            DateTime? onHoldDate;

            if (IsOnHold.IsChecked ?? false){
                onHoldDate = OnHoldDatePicker.SelectedDate;
            }
            else{
                onHoldDate = null;
            }

            return onHoldDate;
        }

        private void IsSettled_OnChecked(object sender, RoutedEventArgs e){
            IsFunded.IsChecked = true;
        }
    }
}
