using EmployeeProject.Model.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Core.ModelView
{
    public  class ApplyLeaveViewModel
    {
        [Required]
        public string LeaveType { get; set; }

        [Required]
        public DateTime? FromDate { get; set; }

        [Required]
        public DateTime? ToDate { get; set; }
        [Required]
        [StringLength(250,ErrorMessage ="Reason cannot exceed 250 characters")]
        public string Reason { get; set; } = string.Empty;

        public IFormFile? Document { get; set; }
    }
}
