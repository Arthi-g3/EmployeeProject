using EmployeeProject.Model;

namespace EmployeeProject.Data
{
    public interface ITimesheetRepository
    {
        Task AddAsync(Timesheet timesheet);
        Task<List<Timesheet>> GetByEmployeeIdAsync(int employeeId);
        Task<List<Timesheet>> GetAllAsync();
        Task<bool> ExistsForEmployeeAndDateAsync(int employeeId, DateTime workDate);
    }
}
