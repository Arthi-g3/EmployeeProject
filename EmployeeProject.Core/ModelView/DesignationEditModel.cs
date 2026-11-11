using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Core.ModelView
{
   public class DesignationEditModel
    {
        public int DesignationId { get; set; }

        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; }
    }
}
