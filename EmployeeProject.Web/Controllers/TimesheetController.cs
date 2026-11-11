using EmployeeProject.Core;
using EmployeeProject.Core.ModelView;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers
{
    [Authorize]
    public class TimesheetController : Controller
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationIdentity> _userManager;

        public TimesheetController(
            ITimesheetService timesheetService,
            IProjectService projectService,
            UserManager<ApplicationIdentity> userManager)
        {
            _timesheetService = timesheetService;
            _projectService = projectService;
            _userManager = userManager;
        }

        private async Task<ApplicationIdentity?> GetCurrentEmployeeAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.EmployeeId == null)
            {
                return null;
            }

            return user;
        }

        private async Task PopulateProjectsAsync()
        {
            ViewBag.Projects = (await _projectService.GetAllAsync())
                .Where(project => project.IsActive)
                .OrderBy(project => project.ProjectName)
                .ToList();
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> MyTimesheets()
        {
            var user = await GetCurrentEmployeeAsync();

            if (user == null)
            {
                return Forbid();
            }

            var timesheets = await _timesheetService
                .GetByEmployeeIdAsync(user.EmployeeId!.Value);

            return View(timesheets);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateProjectsAsync();
            return View(new TimesheetCreateViewModel
            {
                WorkDate = DateTime.Today
            });
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimesheetCreateViewModel model)
        {
            var user = await GetCurrentEmployeeAsync();

            if (user == null)
            {
                return Forbid();
            }

            await PopulateProjectsAsync();

            var projects = (List<Project>)ViewBag.Projects;
            var project = projects.FirstOrDefault(p => p.ProjectId == model.ProjectId);

            if (project == null)
            {
                ModelState.AddModelError(
                    nameof(model.ProjectId),
                    "Please select an active project.");
            }

            if (model.WorkDate.HasValue &&
                await _timesheetService.ExistsForEmployeeAndDateAsync(
                    user.EmployeeId!.Value,
                    model.WorkDate.Value))
            {
                ModelState.AddModelError(
                    nameof(model.WorkDate),
                    "You have already submitted a timesheet for this date.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var timesheet = new Timesheet
            {
                EmployeeId = user.EmployeeId.Value,
                ProjectId = model.ProjectId,
                WorkDate = model.WorkDate!.Value.Date,
                Hours = model.Hours,
                Description = model.Description.Trim(),
                TaskReference = model.TaskReference.Trim()
            };

            if (!await _timesheetService.AddAsync(timesheet))
            {
                ModelState.AddModelError(
                    nameof(model.WorkDate),
                    "A timesheet already exists for this date.");
                return View(model);
            }

            TempData["Success"] = "Timesheet submitted successfully.";
            return RedirectToAction(nameof(MyTimesheets));
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> ManagerTimesheets()
        {
            var timesheets = await _timesheetService.GetAllAsync();
            return View(timesheets);
        }
    }
}
