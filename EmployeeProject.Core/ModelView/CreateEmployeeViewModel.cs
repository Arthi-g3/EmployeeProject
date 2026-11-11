using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmployeeProject.Model.Enums;
namespace EmployeeProject.Core.ModelView
{
    public class CreateEmployeeViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(18, 60)]
        public int Age { get; set; }

        [Required]
        public Gender Gender { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a department")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a designation")]
        public int DesignationId { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "999999999",
            ErrorMessage = "Salary must be greater than zero")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Please select a reporting person")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a reporting person")]
        public int ReportingPersonId { get; set; }
    }
    }
