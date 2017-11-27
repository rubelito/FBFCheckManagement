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

        public string Notes { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public bool IsFunded { get; set; }
        public bool IsSettled { get; set; }

        public DateTime? HoldDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public bool IsOnHold { get { return HoldDate.HasValue; }}

        public string Status{
            get{
                string status = string.Empty;

                if (IsSettled){
                    status = "Settled";
                }
                else if (IsFunded){
                    status = "Funded";
                }
                else if (IsOnHold){
                    status = "On Hold";
                }
                else{
                    // Normal
                    status = "For Funding";
                }

                return status;
            }
        }
    }
}