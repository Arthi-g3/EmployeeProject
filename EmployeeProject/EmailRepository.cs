using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmployeeDbContext _context;
        public EmailRepository(EmployeeDbContext context)
        {
           _context = context; 
        }

        public async Task AddEmailLogAsync(EmailLog email)
        {
            await _context.EmailLogs.AddAsync(email);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmailLogAsync(EmailLog email)
        {
            _context.EmailLogs.Update(email);
            await _context.SaveChangesAsync();
        }
    }
}
