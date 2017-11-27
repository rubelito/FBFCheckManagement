using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.WPF.HelperClass;

namespace FBFCheckManagement.WPF.ViewModel
{
    public class CheckMaintenanceView : BindableBase
    {
        //Search 
        private string _checkNumber;
        public string CheckNumber { get { return _checkNumber; } set { SetProperty(ref _checkNumber, value); } }

        public ICollectionView Departments { get; set; }

        private ICollectionView _banks;
        public ICollectionView Banks { get { return _banks; } set { SetProperty(ref _banks, value); } }

        private bool _hasSelectedDepartment;
        public bool HasSelectedDepartment
        {
            get { return _hasSelectedDepartment; }
            set
            {
                SetProperty(ref _hasSelectedDepartment, value);
            }
        }


        private Department _selectedDepartment;
        public Department SelectedDepartment {get { return _selectedDepartment; } set{
            SetProperty(ref _selectedDepartment, value);
        }}

        private Bank _selectedBank;
        public Bank SelectedBank { get { return _selectedBank; } set { SetProperty(ref _selectedBank, value); } }

        private decimal _amountFrom;
        public decimal AmountFrom { get { return _amountFrom; } set { SetProperty(ref _amountFrom, value); } }
        private decimal _amountTo;
        public decimal AmountTo { get { return _amountTo; } set { SetProperty(ref _amountTo, value); } }

        private  DateTime? _issuedDateFrom;

        public DateTime? IssuedDateFrom{
            get { return _issuedDateFrom; }
            set { SetProperty(ref _issuedDateFrom, value); }
        }

        private DateTime? _issuedDateTo;

        public DateTime? IssuedDateTo{
            get { return _issuedDateTo; }
            set { SetProperty(ref _issuedDateTo, value); }
        }

        private string _issuedTo;
        public string IssuedTo { get { return _issuedTo; } set { SetProperty(ref _issuedTo, value); } }

        private DateTime? _createdDateFrom;

        public DateTime? CreatedDateFrom{
            get { return _createdDateFrom; }
            set { SetProperty(ref _createdDateFrom, value); }
        }

        private DateTime? _createdDateTo;

        public DateTime? CreatedDateTo{
            get { return _createdDateTo; }
            set { SetProperty(ref _createdDateTo, value); }
        }

        public ComboBoxItem SetOrderby { get; set; }

        private OrderByArrangement _selectedOrder;
        public OrderByArrangement SelectedOrder { get { return _selectedOrder; } set{SetProperty(ref _selectedOrder, value);} }

        public OrderBy SelectedOrderBy {
            get { return GetOrderby(); } }

        private OrderBy GetOrderby(){
            OrderBy o = OrderBy.CreatedDate;

            if (SetOrderby.Content.ToString() == "Created Date")
                o = OrderBy.CreatedDate;
            if (SetOrderby.Content.ToString() == "Issued Date")
                o = OrderBy.IssuedDate;
            if (SetOrderby.Content.ToString() == "Check #")
                o = OrderBy.CheckNumber;
            if (SetOrderby.Content.ToString() == "Amount")
                o = OrderBy.Amount;
            if (SetOrderby.Content.ToString() == "Issued To")
                o = OrderBy.IssuedTo;

            return o;
        }

        private ICollectionView _pagedChecks;
        public ICollectionView PagedChecks
        {
            get { return _pagedChecks; }
            set { SetProperty(ref _pagedChecks, value); }
        }

        public Check SelectedCheck { get; set; }
    }
}