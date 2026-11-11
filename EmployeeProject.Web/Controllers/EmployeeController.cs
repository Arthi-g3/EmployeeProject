
using EmployeeProject.Core;
using EmployeeProject.Core.ModelView;
using EmployeeProject.Data;
using EmployeeProject.Model;
using EmployeeProject.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Web.Controllers;

[Route("[controller]/[action]")]
public class EmployeeController : Controller
{
    private readonly UserManager<ApplicationIdentity> _user;
    private readonly EmployeeDbContext _context;
    private readonly IEmailService _service;
    private readonly IEmailRepository _repository;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;

    public EmployeeController(
        UserManager<ApplicationIdentity> userManager,
        EmployeeDbContext context,
        IEmailService service,
        IEmailRepository repository,
        IDesignationService designationService,
        IDepartmentService departmentService)
    {
        _user = userManager;
        _context = context;
        _service = service;
        _repository = repository;
        _designationService = designationService;
        _departmentService = departmentService;
    }

    // GET: Employee/Index
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        int pageSize = 5;

        var employeeQuery = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .AsNoTracking();

        int totalRecords = await employeeQuery.CountAsync();

        int totalPages = (int)Math.Ceiling(
            totalRecords / (double)pageSize);

        if (page < 1)
            page = 1;

        if (totalPages > 0 && page > totalPages)
            page = totalPages;

        var employees = await employeeQuery
            .OrderBy(e => e.EmpId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Departments = await _context.Departments
            .Where(d => !d.IsDeleted)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalRecords = totalRecords;

        return View("~/Views/Employees/Index.cshtml", employees);
    }

    // GET: Employee/CreateEmployee
    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> CreateEmployee()
    {
        await LoadEmployeeDropdowns();

        return View("~/Views/Employee/CreateEmployee.cshtml");
    }

    // POST: Employee/CreateEmployee
    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEmployee(
        CreateEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEmployeeDropdowns(model);
            return View("~/Views/Employee/CreateEmployee.cshtml", model);
        }

        // Validate Reporting Person
        var validReportingPerson = await (
            from reportingEmployee in _context.Employees
            join identityUser in _context.Users
                on (int?)reportingEmployee.EmpId equals identityUser.EmployeeId
            join userRole in _context.UserRoles
                on identityUser.Id equals userRole.UserId
            join role in _context.Roles
                on userRole.RoleId equals role.Id
            where reportingEmployee.EmpId == model.ReportingPersonId
                  && (role.Name == "Admin" || role.Name == "HR")
            select reportingEmployee.EmpId
        ).AnyAsync();

        if (!validReportingPerson)
        {
            ModelState.AddModelError(
                nameof(model.ReportingPersonId),
                "Please select a valid Admin or HR reporting person.");

            await LoadEmployeeDropdowns(model);
            return View("~/Views/Employee/CreateEmployee.cshtml", model);
        }

        // Check existing email
        var existingUser = await _user.FindByEmailAsync(model.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "An account with this email already exists.");

            await LoadEmployeeDropdowns(model);
            return View("~/Views/Employee/CreateEmployee.cshtml", model);
        }

        // Generate temporary password
        var temporaryPassword =
            "Emp@" + Guid.NewGuid().ToString("N")[..12] + "A1!";

        // Create Employee
        var employee = new Employee
        {
            Name = model.Name,
            Age = model.Age,
            Gender = model.Gender,
            Email = model.Email,
            DepartmentId = model.DepartmentId,
            DesignationId = model.DesignationId,
            PhoneNumber = model.PhoneNumber,
            City = model.City,
            Salary = model.Salary,
            ReportingPersonId = model.ReportingPersonId,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Create Identity account
        var user = new ApplicationIdentity
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            EmployeeId = employee.EmpId,
            IsActive = true,
            CreatedDate = DateTime.Now,
            EmailConfirmed = false,
            MustChangePassword = true
        };

        var result = await _user.CreateAsync(user, temporaryPassword);

        if (!result.Succeeded)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            await LoadEmployeeDropdowns(model);
            return View("~/Views/Employee/CreateEmployee.cshtml", model);
        }

        // Assign Employee role
        var roleResult = await _user.AddToRoleAsync(user, "Employee");

        if (!roleResult.Succeeded)
        {
            await _user.DeleteAsync(user);

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            await LoadEmployeeDropdowns(model);
            return View("~/Views/Employee/CreateEmployee.cshtml", model);
        }

        // Create Email Log
        var emailLog = new EmailLog
        {
            EmpId = employee.EmpId,
            From = "admin@employeemanagement.local",
            To = model.Email,
            Subject = "Employee Account Created",
            Status = "Pending",
            CreatedDate = DateTime.Now
        };

        await _repository.AddEmailLogAsync(emailLog);

        // Send temporary password email
        try
        {
            var loginUrl = "https://localhost:7121/Account/Login";

            var emailBody = EmployeeWelcomeTemplate.Generate(
                employee.EmpId,
                model.Name,
                model.Email,
                temporaryPassword,
                loginUrl);

            await _service.SendEmailAsync(
                model.Email,
                emailLog.Subject,
                emailBody);

            emailLog.Status = "Sent";
            emailLog.Error = null;
        }
        catch (Exception ex)
        {
            emailLog.Status = "Failed";
            emailLog.Error = ex.Message;
        }

        // Update Email Log
        emailLog.UpdatedDate = DateTime.Now;

        await _repository.UpdateEmailLogAsync(emailLog);

        TempData["CreatedEmail"] = model.Email;
        TempData["EmailStatus"] = emailLog.Status;

        return RedirectToAction(nameof(EmployeeCreated));
    }

    // GET: Employee/EmployeeCreated
    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult EmployeeCreated()
    {
        return View();
    }

    // GET: Employee/Edit
    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
            return NotFound();

        var model = new EditEmployeeViewModel
        {
            EmpId = employee.EmpId,
            Name = employee.Name,
            Age = employee.Age,
            Gender = employee.Gender,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            ReportingPersonId = employee.ReportingPersonId,
            PhoneNumber = employee.PhoneNumber,
            City = employee.City,
            Salary = employee.Salary,
            CreatedDate = employee.CreatedDate,
            UpdatedDate = employee.UpdatedDate
        };

        await LoadEditDropdowns(model);

        return View("~/Views/Employees/Edit.cshtml", model);
    }

    // POST: Employee/Edit
    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadEditDropdowns(model);
            return View("~/Views/Employees/Edit.cshtml", model);
        }

        var employee = await _context.Employees.FindAsync(model.EmpId);

        if (employee == null)
            return NotFound();

        // Validate Reporting Person
        var validReportingPerson = await (
            from reportingEmployee in _context.Employees
            join identityUser in _context.Users
                on (int?)reportingEmployee.EmpId equals identityUser.EmployeeId
            join userRole in _context.UserRoles
                on identityUser.Id equals userRole.UserId
            join role in _context.Roles
                on userRole.RoleId equals role.Id
            where reportingEmployee.EmpId == model.ReportingPersonId
                  && reportingEmployee.EmpId != model.EmpId
                  && (role.Name == "Admin" || role.Name == "HR")
            select reportingEmployee.EmpId
        ).AnyAsync();

        if (!validReportingPerson)
        {
            ModelState.AddModelError(
                nameof(model.ReportingPersonId),
                "Please select a valid Admin or HR reporting person.");

            await LoadEditDropdowns(model);
            return View("~/Views/Employees/Edit.cshtml", model);
        }

        employee.Name = model.Name;
        employee.Age = model.Age;
        employee.Gender = model.Gender;
        employee.Email = model.Email;
        employee.DepartmentId = model.DepartmentId;
        employee.DesignationId = model.DesignationId;
        employee.ReportingPersonId = model.ReportingPersonId;
        employee.PhoneNumber = model.PhoneNumber;
        employee.City = model.City;
        employee.Salary = model.Salary;
        employee.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Employee updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Employee/Profile
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _user.GetUserAsync(User);

        if (user == null)
            return Challenge();

        if (User.IsInRole("Employee") && user.MustChangePassword)
        {
            return RedirectToAction("ChangePassword", "Password");
        }

        var profile = new ProfileViewModel
        {
            Name = user.UserName,
            Email = user.Email,
            Role = User.IsInRole("Admin") ? "Admin"
                 : User.IsInRole("HR") ? "HR"
                 : User.IsInRole("Employee") ? "Employee"
                 : "User"
        };

        if (user.EmployeeId.HasValue)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(
                    e => e.EmpId == user.EmployeeId.Value);

            if (employee != null)
            {
                profile.Name = employee.Name;
                profile.Email = employee.Email;
                profile.EmployeeId = employee.EmpId;
                profile.Department = employee.Department?.DepartmentName;
                profile.Designation = employee.Designation?.DesignationName;
            }
        }

        return View("Profile", profile);
    }

    // GET: Employee/Delete
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
            return NotFound();

        return View("~/Views/Employees/Delete.cshtml", employee);
    }

    // POST: Employee/Delete
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Employee deleted successfully.";

        return RedirectToAction(nameof(Index));
    }


    // Load Create Employee Dropdowns
    private async Task LoadEmployeeDropdowns(
        CreateEmployeeViewModel? model = null)
    {
        var departments = await _context.Departments
            .Where(d => !d.IsDeleted)
            .ToListAsync();

        ViewBag.Departments = new SelectList(
            departments,
            "Id",
            "DepartmentName",
            model?.DepartmentId);

        var designations = await _context.Designations
            .Where(d => d.IsActive)
            .OrderBy(d => d.DesignationName)
            .Select(d => new
            {
                id = d.DesignationId,
                name = d.DesignationName,
                departmentId = d.DepartmentId
            })
            .ToListAsync();

        ViewBag.DesignationData =
            System.Text.Json.JsonSerializer.Serialize(designations);

        var reportingPersons = await (
            from e in _context.Employees
            join u in _context.Users
                on (int?)e.EmpId equals u.EmployeeId
            join ur in _context.UserRoles
                on u.Id equals ur.UserId
            join r in _context.Roles
                on ur.RoleId equals r.Id
            where r.Name == "Admin" || r.Name == "HR"
            select e
        )
        .Distinct()
        .OrderBy(e => e.Name)
        .ToListAsync();

        ViewBag.ReportingPersons = new SelectList(
            reportingPersons,
            "EmpId",
            "Name",
            model?.ReportingPersonId);
    }

    // Load Edit Employee Dropdowns
    private async Task LoadEditDropdowns(
        EditEmployeeViewModel model)
    {
        var departments = await _context.Departments
            .Where(d => !d.IsDeleted)
            .ToListAsync();

        ViewBag.Departments = new SelectList(
            departments,
            "Id",
            "DepartmentName",
            model.DepartmentId);

        var designations = await _context.Designations
            .Where(d => d.IsActive)
            .OrderBy(d => d.DesignationName)
            .ToListAsync();

        ViewBag.Designations = new SelectList(
            designations,
            "DesignationId",
            "DesignationName",
            model.DesignationId);

        var reportingPersons = await (
            from e in _context.Employees
            join u in _context.Users
                on (int?)e.EmpId equals u.EmployeeId
            join ur in _context.UserRoles
                on u.Id equals ur.UserId
            join r in _context.Roles
                on ur.RoleId equals r.Id
            where e.EmpId != model.EmpId
                  && (r.Name == "Admin" || r.Name == "HR")
            select e
        )
        .Distinct()
        .OrderBy(e => e.Name)
        .ToListAsync();

        ViewBag.ReportingPersons = new SelectList(
            reportingPersons,
            "EmpId",
            "Name",
            model.ReportingPersonId);
    }
}
