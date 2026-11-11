using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProject.Model
{
    public class Designation
    {
        public int DesignationId { get; set; }

        // Department relationship
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
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