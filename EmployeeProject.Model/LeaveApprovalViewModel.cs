using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Model
{
    public class LeaveApprovalViewModel
    {
        public int LeaveId { get; set; }

        public string Status { get; set; } = "Pending";

        public string? ApproverRemarks { get; set; }
    }
}
