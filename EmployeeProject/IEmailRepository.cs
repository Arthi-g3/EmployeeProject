using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public interface IEmailRepository
    {
        Task AddEmailLogAsync(EmailLog email);
        Task UpdateEmailLogAsync(EmailLog email);
    }
}
