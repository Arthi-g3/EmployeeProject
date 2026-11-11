using EmployeeProject.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeProject.Data
{
    public interface ILeaveRepository
    {
        Task AddLeaveAsync(EmployeeLeaveDetail leave);

        Task<List<EmployeeLeaveDetail>> GetEmployeeLeavesAsync(
            int employeeId);

        Task<EmployeeLeaveDetail?> GetLeaveByIdAsync(int leaveId);

        Task UpdateLeaveAsync(EmployeeLeaveDetail leave);

        Task SyncPendingLeavesAsync();

        Task<LeaveListViewModel> GetReportingPersonLeavesAsync(
            int reportingPersonId,
            string status,
            string? search,
            int page,
            int pageSize);

        Task<List<EmployeeLeaveDetail>> GetPendingLeavesAsync(
            int reportingPersonId,
            string status,
            int page,
            int pageSize);

        Task<int> GetPendingLeavesCountAsync(
            int reportingPersonId,
            string status);
    }
}