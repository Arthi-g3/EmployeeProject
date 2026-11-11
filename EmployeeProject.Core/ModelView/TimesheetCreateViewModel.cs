using System.ComponentModel.DataAnnotations;

namespace EmployeeProject.Core.ModelView
{
    public class TimesheetCreateViewModel
    {
        [Required(ErrorMessage = "Work date is required.")]
        [DataType(DataType.Date)]
        public DateTime? WorkDate { get; set; }

        [Range(1, 8, ErrorMessage = "Hours must be between 1 and 8.")]
        public decimal Hours { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a project.")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task reference is required.")]
        [StringLength(100, ErrorMessage = "Task reference cannot exceed 100 characters.")]
        public string TaskReference { get; set; } = string.Empty;
    }
}
