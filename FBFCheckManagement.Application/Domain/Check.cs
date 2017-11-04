using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBFCheckManagement.Application.Domain
{
    public class Check
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string CheckNumber { get; set; }

        [Required]
        public virtual Bank Bank { get; set; }
        public DateTime? DateIssued { get; set; }
        [Required]
        public string IssuedTo { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public bool IsFunded { get; set; }

        public DateTime? HoldDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsOnHold { get { return HoldDate.HasValue; }}
    }
}