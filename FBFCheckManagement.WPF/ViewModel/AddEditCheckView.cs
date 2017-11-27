using System;
using System.Collections.Generic;
using System.ComponentModel;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.WPF.HelperClass;

namespace FBFCheckManagement.WPF.ViewModel
{
    public class AddEditCheckView : BindableBase
    {
        public Check Check { get; set; }
        public long CheckToEdit { get; set; }

        public Operation Operation { get; set; }

        public string OperationMode{
            get{
                if (Operation == Operation.Add)
                    return "Add Check";

                 return "Edit Check";
                }
            }

        private Department _selectedDepartment;
        public Department SelectedDepartment
        {
            get { return _selectedDepartment; }
            set
            {
                SetProperty(ref _selectedDepartment, value);
            }
        }

        private Bank _selectedBank;
        public Bank SelectedBank { get { return _selectedBank; } set { SetProperty(ref _selectedBank, value); } }

        private ICollectionView _departments;
        public ICollectionView Departments { get { return _departments; } set { SetProperty(ref _departments, value); } }

        private ICollectionView _banks;
        public ICollectionView Banks { get { return _banks; } set { SetProperty(ref _banks, value); } }

    }
}