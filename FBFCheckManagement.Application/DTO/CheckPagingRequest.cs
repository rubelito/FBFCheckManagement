using System;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.DTO
{
    public class CheckPagingRequest
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
      
        public SearchCriteria SearchCriteria { get; set; }
    }
}