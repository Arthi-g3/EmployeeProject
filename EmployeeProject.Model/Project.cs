using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Model
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100)]
        public string ProjectName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public ICollection<EmployeeTask> EmployeeTasks { get; set; }
            = new List<EmployeeTask>();

        public ICollection<Timesheet> Timesheets { get; set; }
            = new List<Timesheet>();

    }
}
