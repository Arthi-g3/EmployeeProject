using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Model
{
    public class EmployeeLeaveDetail
    {
        public int LeaveId { get; set; }

            public int EmployeeId { get; set; }

            public string LeaveType { get; set; }

            public DateTime FromDate { get; set; }

            public DateTime ToDate { get; set; }

            public int TotalDays { get; set; }

            public string Reason { get; set; }

            public string? DocumentPath { get; set; }

            public int ReportingPersonId { get; set; }

            public string Status { get; set; } = "Pending";

            public string? ApproverRemarks { get; set; }

            public DateTime CreatedDate { get; set; } = DateTime.Now;

            public DateTime? UpdatedDate { get; set; }
        public Employee Employee { get; set; } = null!;
    }
    }

