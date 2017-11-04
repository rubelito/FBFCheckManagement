using System;
using System.Collections.Generic;
using System.ComponentModel;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.WPF.ViewModel
{
    public class AddEditCheckView
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

        public  List<Bank> Banks { get; set; }
    }
}