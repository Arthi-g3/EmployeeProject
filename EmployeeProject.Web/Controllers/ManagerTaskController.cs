using EmployeeProject.Core;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerTaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IEmployeeService _employeeService;
    private readonly IProjectService _projectService;

    public ManagerTaskController(
        ITaskService taskService,
        IEmployeeService employeeService,
        IProjectService projectService)
    {
        _taskService = taskService;
        _employeeService = employeeService;
        _projectService = projectService;
    }

    private async Task<(List<Employee> Employees, List<Project> Projects)> GetAssignmentOptionsAsync()
    {
        var employees = (await _employeeService.GetEmployeesAsync())
            .Where(e => e.IsActive)
            .OrderBy(e => e.Name)
            .ToList();

        var projects = (await _projectService.GetAllAsync())
            .Where(p => p.IsActive)
            .OrderBy(p => p.ProjectName)
            .ToList();

        return (employees, projects);
    }

    private async Task PopulateAssignmentOptionsAsync()
    {
        var options = await GetAssignmentOptionsAsync();
        ViewBag.Employees = options.Employees;
        ViewBag.Projects = options.Projects;
    }

    [HttpGet]
    public async Task<IActionResult> Assign()
    {
        await PopulateAssignmentOptionsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(EmployeeTask task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            ModelState.AddModelError(nameof(task.Title), "Task title is required.");
        }

        var options = await GetAssignmentOptionsAsync();
        var employeeExists = options.Employees.Any(e => e.EmpId == task.EmpId);
        var projectExists = task.ProjectId.HasValue &&
                            options.Projects.Any(p => p.ProjectId == task.ProjectId.Value);

        if (!employeeExists)
        {
            ModelState.AddModelError(nameof(task.EmpId), "Select an active employee.");
        }

        if (!projectExists)
        {
            ModelState.AddModelError(nameof(task.ProjectId), "Select an active project.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Employees = options.Employees;
            ViewBag.Projects = options.Projects;
            return View(task);
        }

        task.Status = EmployeeProject.Model.Enums.EmployeeStatus.Pending;
        task.CreatedDate = DateTime.Now;

        await _taskService.AddTaskAsync(task);

        TempData["Success"] = "Task assigned successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return View(tasks);
    }
}
