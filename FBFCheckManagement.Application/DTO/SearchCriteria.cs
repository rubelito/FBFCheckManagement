using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.DTO
{
    public class SearchCriteria
    {
        public string CheckNumber { get; set; }

        public Bank SelectedBank { get; set; }

        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }

        public DateTime? IssuedDateFrom { get; set; }
        public DateTime? IssuedDateTo { get; set; }

        public string IssuedTo { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }

        public OrderByArrangement Order { get; set; }
        public OrderBy OrderBy { get; set; }
    }
}
