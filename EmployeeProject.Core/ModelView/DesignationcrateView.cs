using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Core.ModelView
{
    public class DesignationcrateView
    {
        public int DesignationId { get; set; }

        // Department relationship
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        // Employee relationship
        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}
