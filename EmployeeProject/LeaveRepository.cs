
using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeProject.Data
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly EmployeeDbContext _context;

        public LeaveRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        // Add Leave
        public async Task AddLeaveAsync(EmployeeLeaveDetail leave)
        {
            await _context.EmployeeLeaveDetails.AddAsync(leave);
            await _context.SaveChangesAsync();

            var pendingLeave = new PendingLeave
            {
                LeaveId = leave.LeaveId,
                ReportingPersonId = leave.ReportingPersonId,
                PendingStatus = "Pending",
                SubmittedDate = DateTime.Now
            };

            await _context.PendingLeaves.AddAsync(pendingLeave);
            await _context.SaveChangesAsync();
        }

        // Get Employee Leaves
        public async Task<List<EmployeeLeaveDetail>> GetEmployeeLeavesAsync(
            int employeeId)
        {
            return await _context.EmployeeLeaveDetails
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();
        }

        // Get Leave By Id
        public async Task<EmployeeLeaveDetail?> GetLeaveByIdAsync(int leaveId)
        {
            return await _context.EmployeeLeaveDetails
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        // Update Leave
        public async Task UpdateLeaveAsync(EmployeeLeaveDetail leave)
        {
            var pendingLeave = await _context.PendingLeaves
                .FirstOrDefaultAsync(p => p.LeaveId == leave.LeaveId);

            if (pendingLeave != null)
            {
                pendingLeave.PendingStatus = leave.Status;
            }

            _context.EmployeeLeaveDetails.Update(leave);

            await _context.SaveChangesAsync();
        }

        // Sync Pending Leaves
        public async Task SyncPendingLeavesAsync()
        {
            var pendingLeaves = await _context.EmployeeLeaveDetails
                .Where(l => l.Status == "Pending")
                .ToListAsync();

            foreach (var leave in pendingLeaves)
            {
                bool alreadyExists = await _context.PendingLeaves
                    .AnyAsync(p => p.LeaveId == leave.LeaveId);

                if (!alreadyExists)
                {
                    var pendingLeave = new PendingLeave
                    {
                        LeaveId = leave.LeaveId,
                        ReportingPersonId = leave.ReportingPersonId,
                        PendingStatus = "Pending",
                        SubmittedDate = DateTime.Now
                    };

                    await _context.PendingLeaves.AddAsync(pendingLeave);
                }
            }

            await _context.SaveChangesAsync();
        }

        // Get Reporting Person Leaves
        public async Task<LeaveListViewModel> GetReportingPersonLeavesAsync(
            int reportingPersonId,
            string status,
            string? search,
            int page,
            int pageSize)
        {
            var query = _context.EmployeeLeaveDetails
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Department)
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Designation)
                .Where(l =>
                    l.ReportingPersonId == reportingPersonId &&
                    l.Status == status);

            // Search by employee name or employee ID
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(l =>
                    l.Employee.Name.Contains(search) ||
                    l.Employee.EmpId.ToString().Contains(search));
            }

            int totalRecords = await query.CountAsync();

            var leaves = await query
                .OrderByDescending(l => l.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LeaveApprovalViewModel
                {
                    LeaveId = l.LeaveId,
                    Status = l.Status,
                    ApproverRemarks = l.ApproverRemarks
                })
                .ToListAsync();

            return new LeaveListViewModel
            {
                Leaves = leaves,
                Status = status,
                Search = search,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        
        // Get Reporting Person Leaves with Status Filter and Pagination
        public async Task<List<EmployeeLeaveDetail>> GetPendingLeavesAsync(
            int reportingPersonId,
            string status,
            int page,
            int pageSize)
        {
            var query = _context.EmployeeLeaveDetails
                .Include(l => l.Employee)
                .Where(l => l.ReportingPersonId == reportingPersonId);

            // Filter by status
            if (status != "All")
            {
                query = query.Where(l => l.Status == status);
            }

            return await query
                .OrderByDescending(l => l.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        // Get Reporting Person Leaves Count
        public async Task<int> GetPendingLeavesCountAsync(
            int reportingPersonId,
            string status)
        {
            var query = _context.EmployeeLeaveDetails
                .Where(l => l.ReportingPersonId == reportingPersonId);

            if (status != "All")
            {
                query = query.Where(l => l.Status == status);
            }

            return await query.CountAsync();
        }
    }
}
