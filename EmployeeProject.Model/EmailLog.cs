using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Model
{
    public class EmailLog
    {
        public int Id { get; set; }

        public int EmpId { get; set; }

        public string From { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? Error { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
