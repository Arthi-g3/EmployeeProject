
using EmployeeProject.Core;
using EmployeeProject.Core.Services;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;

            if (page < 1)
                page = 1;

            var result = await _departmentService.GetPagedAsync(page, pageSize);

            int totalPages = (int)Math.Ceiling(
                result.TotalRecords / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            // Fetch again if the requested page was beyond the last page.
            if (page != 1 && page > totalPages)
                page = totalPages;

            result = await _departmentService.GetPagedAsync(page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = result.TotalRecords;

            return View(result.Departments);
        }

        // Create - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter valid department details.";
                return View(model);
            }

            bool created = await _departmentService.CreateAsync(model);

            if (!created)
            {
                TempData["Error"] =
                    "Department name is empty or already exists.";

                return View(model);
            }

            TempData["Success"] = "Department created successfully!";

            return RedirectToAction(nameof(Index));
        }

        // Edit - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter a valid department name.";
                return View(model);
            }

            bool updated = await _departmentService.UpdateAsync(model);

            if (!updated)
            {
                TempData["Error"] =
                    "Department not found or the name already exists.";

                return View(model);
            }

            TempData["Success"] = "Department updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // Delete - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // Delete - POST
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _departmentService.DeleteAsync(id);

            if (!deleted)
            {
                TempData["Error"] = "Department not found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Department deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
