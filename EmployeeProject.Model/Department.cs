using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeProject.Model
{

    
        public class Department
        {
        public int Id { get; set;  }

            [Required]
            [StringLength(100)]
            public string DepartmentName { get; set; } = string.Empty;

            public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

            public DateTime? UpdatedDate { get; set; }

            public bool IsDeleted { get; set; }

            public ICollection<Employee> Employees { get; set; }
                = new List<Employee>();

        //designation
        public ICollection<Designation> Designations { get; set; } = new List<Designation>();
        }
    }

