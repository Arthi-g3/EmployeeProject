using System.ComponentModel.DataAnnotations;
using EmployeeProject.Model.Enums;

namespace EmployeeProject.Model
{
    public class Timesheet
    {
        public int TimesheetId { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public DateTime WorkDate { get; set; }

        [Range(1, 8)]
        public decimal Hours { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TaskReference { get; set; } = string.Empty;

        public TimesheetStatus Status { get; set; } = TimesheetStatus.Submitted;

        public DateTime SubmittedDate { get; set; } = DateTime.Now;
    }
}
