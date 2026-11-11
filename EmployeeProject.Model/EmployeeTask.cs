using System;
using System.Collections.Generic;
using System.Text;

using EmployeeProject.Model.Enums;

namespace EmployeeProject.Model
{
    public class EmployeeTask
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int EmpId { get; set; }
        //poject relationship
        public int? ProjectId { get; set; }

        public Project? Project { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
        public EmployeeStatus Status { get; set; }

    }
}
