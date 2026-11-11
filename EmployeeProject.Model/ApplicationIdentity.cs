
using Microsoft.AspNetCore.Identity;
using System;


namespace EmployeeProject.Model
{
    public class ApplicationIdentity : IdentityUser
    {
        public int? EmployeeId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
        public bool MustChangePassword { get; set; } = true;
    }
}
