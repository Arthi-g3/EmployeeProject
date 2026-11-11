using System;
using System.Collections.Generic;

namespace EmployeeProject.Model
{
    public class LeaveListViewModel
    {
        public List<LeaveApprovalViewModel> Leaves { get; set; } = new();

        public string Status { get; set; } = "Pending";

        public string? Search { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages =>
            PageSize > 0
            ? (int)Math.Ceiling((double)TotalRecords / PageSize)
            : 0;
    }
}