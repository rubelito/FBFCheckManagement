using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBFCheckManagement.Application.Domain
{
    public class Bank
    {
        private List<Check> _checks; 

        public Bank(){
            _checks = new List<Check>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string BankName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        [Required]
        public virtual Department Department { get; set; }

        public virtual List<Check> Checks { get { return _checks; } set { _checks = value; } } 
    }
}
