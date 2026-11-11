using EmployeeProject.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeProject.Core
{
    public interface ILeaveService
    {
        Task ApplyLeaveAsync(EmployeeLeaveDetail leave);

        Task<List<EmployeeLeaveDetail>> GetEmployeeLeavesAsync(
            int employeeId);

        Task<List<EmployeeLeaveDetail>> GetPendingLeavesAsync(
            int reportingPersonId,
            string status,
            int page,
            int pageSize);

        Task<int> GetPendingLeavesCountAsync(
            int reportingPersonId,
            string status);

        Task ApproveOrRejectLeaveAsync(
            int leaveId,
            string status,
            string? remarks);

        Task<EmployeeLeaveDetail?> GetLeaveByIdAsync(int leaveId);

        Task SyncPendingLeavesAsync();

        Task UpdateLeaveAsync(EmployeeLeaveDetail leave);

        Task<LeaveListViewModel> GetReportingPersonLeavesAsync(
            int reportingPersonId,
            string status,
            string? search,
            int page,
            int pageSize);
    }
}