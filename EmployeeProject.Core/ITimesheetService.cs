using EmployeeProject.Model;

namespace EmployeeProject.Core
{
    public interface ITimesheetService
    {
        Task<bool> AddAsync(Timesheet timesheet);
        Task<List<Timesheet>> GetByEmployeeIdAsync(int employeeId);
        Task<List<Timesheet>> GetAllAsync();
        Task<bool> ExistsForEmployeeAndDateAsync(int employeeId, DateTime workDate);
    }
}
