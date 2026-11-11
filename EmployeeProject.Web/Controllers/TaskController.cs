
using EmployeeProject.Core;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers;

[Authorize(Roles = "Employee")]
public class TaskController : Controller
{
    private readonly ITaskService _service;
    private readonly UserManager<ApplicationIdentity> _userManager;

    public TaskController(
        ITaskService service,
        UserManager<ApplicationIdentity> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    private async Task<ApplicationIdentity?> GetCurrentEmployee()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null || user.EmployeeId == null)
            return null;

        return user;
    }

    // LIST
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        var tasks = await _service
            .GetTasksByEmployeeIdAsync(user.EmployeeId!.Value);

        return View(tasks);
    }

    // CREATE - GET
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // CREATE - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeTask task)
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        if (!ModelState.IsValid)
            return View(task);

        task.EmpId = user.EmployeeId!.Value;
        task.CreatedDate = DateTime.Now;

        await _service.AddTaskAsync(task);

        return RedirectToAction(nameof(Index));
    }

    // VIEW DETAILS
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        var task = await _service.GetTaskByIdAsync(id);

        if (task == null || task.EmpId != user.EmployeeId!.Value)
            return NotFound();

        return View(task);
    }

    // EDIT - GET
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        var task = await _service.GetTaskByIdAsync(id);

        if (task == null || task.EmpId != user.EmployeeId!.Value)
            return NotFound();

        return View(task);
    }

    // UPDATE - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeTask task)
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        var existingTask = await _service.GetTaskByIdAsync(id);

        if (existingTask == null ||
            existingTask.EmpId != user.EmployeeId!.Value)
            return NotFound();

        if (!ModelState.IsValid)
            return View(existingTask);

        // Keep the original task ID, owner, and creation date.
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Status = task.Status;

        await _service.UpdateTaskAsync(existingTask);

        TempData["Success"] = "Task updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    // DELETE - CONFIRMATION PAGE
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        var task = await _service.GetTaskByIdAsync(id);

        if (task == null || task.EmpId != user.EmployeeId!.Value)
            return NotFound();

        return View(task);
    }

    // DELETE - POST
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await GetCurrentEmployee();

        if (user == null)
            return RedirectToAction("AccessDenied", "Account");

        var task = await _service.GetTaskByIdAsync(id);

        if (task == null || task.EmpId != user.EmployeeId!.Value)
            return NotFound();

        await _service.DeleteTaskAsync(id);

        TempData["Success"] = "Task deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
    
}
