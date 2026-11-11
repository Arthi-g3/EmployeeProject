
using EmployeeProject.Core;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // Project List
        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllAsync();

            return View(projects);
        }

        // Create GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (project.EndDate < project.StartDate)
            {
                ModelState.AddModelError(
                    nameof(project.EndDate),
                    "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            project.CreatedDate = DateTime.Now;
            project.IsActive = true;

            await _projectService.AddAsync(project);

            TempData["Success"] = "Project created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Edit GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Project project)
        {
            if (project.EndDate < project.StartDate)
            {
                ModelState.AddModelError(
                    nameof(project.EndDate),
                    "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
                return View(project);
            }

            var existingProject =
                await _projectService.GetByIdAsync(project.ProjectId);

            if (existingProject == null)
            {
                return NotFound();
            }

          existingProject. ProjectName = project.ProjectName;
         existingProject. Description = project.Description;
            existingProject.StartDate = project.StartDate;
         existingProject.EndDate = project.EndDate;
            existingProject.IsActive = project.IsActive;
             existingProject.UpdatedDate = DateTime.Now;

            await _projectService.UpdateAsync(existingProject);

            TempData["Success"] = "Project updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }
            if (project.EmployeeTasks.Any())
            {
                TempData["Error"] =
                    "Cannot delete this project because tasks are assigned to it.";

                return RedirectToAction(nameof(Index));
            }


            await _projectService.DeleteAsync(id);

            TempData["Success"] = "Project deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
