using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Model
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;

        public int SmtpPort { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public string SenderEmail { get; set; } = string.Empty;
    }
}
