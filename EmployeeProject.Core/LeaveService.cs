using EmployeeProject.Data;
using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeProject.Core
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repository;

        public LeaveService(ILeaveRepository repository)
        {
            _repository = repository;
        }

        // Apply Leave
        public async Task ApplyLeaveAsync(EmployeeLeaveDetail leave)
        {
            leave.Status = "Pending";
            await _repository.AddLeaveAsync(leave);
        }

        // Get Employee Leaves
        public async Task<List<EmployeeLeaveDetail>> GetEmployeeLeavesAsync(
            int employeeId)
        {
            return await _repository.GetEmployeeLeavesAsync(employeeId);
        }

        // Get Pending Leaves with Status Filter and Pagination
        public async Task<List<EmployeeLeaveDetail>> GetPendingLeavesAsync(
            int reportingPersonId,
            string status,
            int page,
            int pageSize)
        {
            return await _repository.GetPendingLeavesAsync(
                reportingPersonId,
                status,
                page,
                pageSize);
        }

        // Get Pending Leaves Count
        public async Task<int> GetPendingLeavesCountAsync(
            int reportingPersonId,
            string status)
        {
            return await _repository.GetPendingLeavesCountAsync(
                reportingPersonId,
                status);
        }

        // Approve or Reject Leave
        public async Task ApproveOrRejectLeaveAsync(
            int leaveId,
            string status,
            string? remarks)
        {
            var leave = await _repository.GetLeaveByIdAsync(leaveId);

            if (leave == null)
                throw new Exception("Leave record not found.");

            leave.Status = status;
            leave.ApproverRemarks = remarks;
            leave.UpdatedDate = DateTime.Now;

            await _repository.UpdateLeaveAsync(leave);
        }

        // Get Leave By Id
        public async Task<EmployeeLeaveDetail?> GetLeaveByIdAsync(int leaveId)
        {
            return await _repository.GetLeaveByIdAsync(leaveId);
        }

        // Sync Pending Leaves
        public async Task SyncPendingLeavesAsync()
        {
            await _repository.SyncPendingLeavesAsync();
        }

        // Update Leave
        public async Task UpdateLeaveAsync(EmployeeLeaveDetail leave)
        {
            await _repository.UpdateLeaveAsync(leave);
        }

        // Get Reporting Person Leaves
        public async Task<LeaveListViewModel> GetReportingPersonLeavesAsync(
            int reportingPersonId,
            string status,
            string? search,
            int page,
            int pageSize)
        {
            return await _repository.GetReportingPersonLeavesAsync(
                reportingPersonId,
                status,
                search,
                page,
                pageSize);
        }
    }
}