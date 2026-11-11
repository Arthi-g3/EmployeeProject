
using EmployeeProject.Core;
using EmployeeProject.Core.ModelView;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Web.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly EmployeeDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LeaveController(
            ILeaveService leaveService,
            EmployeeDbContext context,
            IWebHostEnvironment environment)
        {
            _leaveService = leaveService;
            _context = context;
            _environment = environment;
        }

        // ==========================================
        // EMPLOYEE - APPLY LEAVE GET
        // ==========================================

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public IActionResult ApplyLeave()
        {
            return View(new ApplyLeaveViewModel());
        }

        // ==========================================
        // EMPLOYEE - APPLY LEAVE POST
        // ==========================================

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(
            ApplyLeaveViewModel model)
        {
            // Validate Leave Type
            string[] allowedLeaveTypes =
            {
                "Sick",
                "Casual",
                "Earned"
            };

            // Validate Date Range
            if (!model.FromDate.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.FromDate),
                    "Please select From Date.");
            }

            if (!model.ToDate.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.ToDate),
                    "Please select To Date.");
            }

            // Validate dates when both are selected
            if (model.FromDate.HasValue && model.ToDate.HasValue)
            {
                if (model.ToDate.Value.Date < model.FromDate.Value.Date)
                {
                    ModelState.AddModelError(
                        nameof(model.ToDate),
                        "To Date cannot be earlier than From Date.");
                }

                // Sick Leave must start today or in future
                if (model.LeaveType == "Sick" &&
                    model.FromDate.Value.Date < DateTime.Today)
                {
                    ModelState.AddModelError(
                        nameof(model.FromDate),
                        "Sick Leave must start from today or a future date.");
                }
            }

            // Calculate Total Days
            int totalDays = 0;

            if (model.FromDate.HasValue &&
                model.ToDate.HasValue &&
                model.ToDate.Value.Date >= model.FromDate.Value.Date)
            {
                totalDays =
                    (model.ToDate.Value.Date - model.FromDate.Value.Date).Days + 1;
            }

            // Document required for Sick Leave of 2 days or more
            bool requiresDocument =
                model.LeaveType == "Sick" && totalDays >= 2;

            if (requiresDocument && model.Document == null)
            {
                ModelState.AddModelError(
                    nameof(model.Document),
                    "Medical proof is required for Sick Leave of 2 days or more.");
            }

            // Validate document file
            if (model.Document != null && requiresDocument)
            {
                string extension =
                    Path.GetExtension(model.Document.FileName)
                    .ToLowerInvariant();

                string[] allowedExtensions =
                {
                    ".pdf", ".jpg", ".jpeg", ".png" 
                };



                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        nameof(model.Document),
                        "Only PDF, JPG, JPEG and PNG files are allowed.");
                }

                if (model.Document.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        nameof(model.Document),
                        "Document size cannot exceed 5 MB.");
                }
            }

            // Check Model Validation
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get Employee ID
            var empIdClaim = User.FindFirst("EmpId");

            if (empIdClaim == null ||
                !int.TryParse(empIdClaim.Value, out int employeeId))
            {
                return Forbid();
            }

            // Get Employee Details
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmpId == employeeId);

            if (employee == null ||
                employee.ReportingPersonId == null)
            {
                ModelState.AddModelError(
                    "",
                    "Reporting person is not assigned.");

                return View(model);
            }

            string? documentPath = null;

            // Upload Document
            if (requiresDocument && model.Document != null)
            {
                string folder = Path.Combine(
                    _environment.WebRootPath,
                    "LeaveDocuments");

                Directory.CreateDirectory(folder);

                string extension =
                    Path.GetExtension(model.Document.FileName)
                    .ToLowerInvariant();

                string fileName = $"{Guid.NewGuid()}{extension}";

                string filePath = Path.Combine(folder, fileName);

                try
                {
                    using (var stream = new FileStream(
                        filePath,
                        FileMode.CreateNew))
                    {
                        await model.Document.CopyToAsync(stream);
                    }

                    documentPath = "/LeaveDocuments/" + fileName;
                }
                catch
                {
                    ModelState.AddModelError(
                        nameof(model.Document),
                        "Unable to upload document. Please try again.");

                    return View(model);
                }
            }

            // Create Leave Record
            var leave = new EmployeeLeaveDetail
            {
                EmployeeId = employeeId,
                LeaveType = model.LeaveType,
                FromDate = model.FromDate!.Value.Date,
                ToDate = model.ToDate!.Value.Date,
                TotalDays = totalDays,
                Reason = model.Reason.Trim(),
                DocumentPath = documentPath,
                ReportingPersonId =
                    employee.ReportingPersonId.Value,
                Status = "All",
                CreatedDate = DateTime.Now
            };

            await _leaveService.ApplyLeaveAsync(leave);

            TempData["SuccessMessage"] ="Leave application submitted successfully.";

            return RedirectToAction(nameof(MyLeaves));
        }

        // ==========================================
        // EMPLOYEE - MY LEAVES
        // ==========================================

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> MyLeaves()
        {
            var empIdClaim = User.FindFirst("EmpId");

            if (empIdClaim == null ||
                !int.TryParse(empIdClaim.Value, out int employeeId))
            {
                return Forbid();
            }

            var leaves =
                await _leaveService.GetEmployeeLeavesAsync(employeeId);

            return View(leaves);
        }

        // ==========================================
        // REPORTING PERSON - PENDING LEAVES
        // ==========================================

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> PendingLeaves(
            string status = "All",
            int page = 1)
        {
            await _leaveService.SyncPendingLeavesAsync();

            var empIdClaim = User.FindFirst("EmpId");

            if (empIdClaim == null ||
                !int.TryParse(empIdClaim.Value, out int reportingPersonId))
            {
                return Forbid();
            }

            int pageSize = 5;

            int totalRecords =
                await _leaveService.GetPendingLeavesCountAsync(
                    reportingPersonId,
                    status);

            int totalPages =
                (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (page < 1)
                page = 1;

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var leaves =
                await _leaveService.GetPendingLeavesAsync(
                    reportingPersonId,
                    status,
                    page,
                    pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SelectedStatus = status;

            return View(leaves);
        }

        // ==========================================
        // REPORTING PERSON - APPROVE / REJECT
        // ==========================================

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReject(
            LeaveApprovalViewModel model)
        {
            var empIdClaim = User.FindFirst("EmpId");

            if (empIdClaim == null ||
                !int.TryParse(empIdClaim.Value, out int reportingPersonId))
            {
                return Forbid();
            }

            if (model.Status != "Approved" &&
                model.Status != "Rejected")
            {
                return BadRequest("Invalid leave status.");
            }

            var leave =
                await _leaveService.GetLeaveByIdAsync(model.LeaveId);

            if (leave == null)
                return NotFound();

            // Verify Reporting Person
            if (leave.ReportingPersonId != reportingPersonId)
                return Forbid();

            // Only Pending Leave Can Be Processed
            if (!string.Equals(
                leave.Status?.Trim(),
                "Pending",
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(
                    "This leave has already been processed.");
            }

            // Update Approval Details
            leave.Status = model.Status;
            leave.ApproverRemarks = model.ApproverRemarks;
            leave.UpdatedDate = DateTime.Now;

            await _leaveService.UpdateLeaveAsync(leave);

            TempData["SuccessMessage"] =
                $"Leave {model.Status.ToLower()} successfully.";

            return RedirectToAction(nameof(PendingLeaves));
        }
    }
}
