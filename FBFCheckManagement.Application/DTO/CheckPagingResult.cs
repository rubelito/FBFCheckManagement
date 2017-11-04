using System.Collections.Generic;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.DTO
{
    public class CheckPagingResult
    {
        public int TotalItems { get; set; }
        public int PageCount { get; set; }
        public List<Check> Results { get; set; } 
    }
}