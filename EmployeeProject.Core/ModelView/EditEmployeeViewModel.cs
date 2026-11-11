using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmployeeProject.Model.Enums;

namespace EmployeeProject.Core.ModelView
{
    public class EditEmployeeViewModel
    {
        public int EmpId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2,
               ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; }
        [Range(18, 60, ErrorMessage = "Age should 18 to 60")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a department")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a designation")]
        public int DesignationId { get; set; }

        [Required(ErrorMessage = "phone number is Required")]
        [RegularExpression(@"^[6-9]\d{9}$",
          ErrorMessage = "Enter a valid 10-digit phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Enter your Salary")]
        public decimal Salary { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int? ReportingPersonId { get; set; }
    }
}
