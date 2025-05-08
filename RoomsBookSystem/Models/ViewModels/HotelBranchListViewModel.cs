using System.Collections.Generic;
using Models;

namespace RoomsBookSystem.Models.ViewModels
{
    public class HotelBranchListViewModel
    {
        public IEnumerable<HotelBranch> HotelBranches { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public int TotalItems { get; set; }
    }
} 