using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Data
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly EmployeeDbContext _context;

        public TimesheetRepository(EmployeeDbContext context)
        {
            _context = context;
        }public async Task<List<Timesheet>> GetByEmployeeIdAsync(int employeeId)
{
    return await _context.Timesheets
        .Include(t => t.Project)
        .Where(t => t.EmployeeId == employeeId)
        .OrderByDescending(t => t.WorkDate)
        .ThenByDescending(t => t.TimesheetId)
        .ToListAsync();
}

        public async Task AddAsync(Timesheet timesheet)
        {
            await _context.Timesheets.AddAsync(timesheet);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Timesheet>> GetByEmployeeIdAsync(int employeeId)
        {
            
                .Include(t => t.Project)
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.WorkDate)
                .ThenByDescending(t => t.TimesheetId)
                .ToListAsync();
        }

        public async Task<List<Timesheet>> GetAllAsync()
        {
            return await _context.Timesheets
                .Include(t => t.Employee)
                .Include(t => t.Project)
                .OrderByDescending(t => t.WorkDate)
                .ThenByDescending(t => t.TimesheetId)
                .ToListAsync();
        }

        public Task<bool> ExistsForEmployeeAndDateAsync(int employeeId, DateTime workDate)
        {
            var date = workDate.Date;
            var nextDate = date.AddDays(1);

            return _context.Timesheets.AnyAsync(t =>
                t.EmployeeId == employeeId &&
                t.WorkDate >= date &&
                t.WorkDate < nextDate);
        }
    }
}
