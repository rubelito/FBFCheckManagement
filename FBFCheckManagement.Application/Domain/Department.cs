using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBFCheckManagement.Application.Domain
{
    public class Department
    {
        private List<Bank> _banks;

        public Department()
        {
            _banks = new List<Bank>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual List<Bank> Banks { get { return _banks; } set { _banks = value; } }
    }
}