using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Model
{
    public class PendingLeave
    {
        public int PendingLeaveId { get; set; }

        public int LeaveId { get; set; }

        public int ReportingPersonId { get; set; }

        public string PendingStatus { get; set; } = "Pending";

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public EmployeeLeaveDetail EmployeeLeaveDetail { get; set; }
    }
}
