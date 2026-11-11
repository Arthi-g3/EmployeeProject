
using EmployeeProject.Core;
using EmployeeProject.Core.ModelView;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeProject.Web.Controllers
{
    public class DesignationController : Controller
    {
        private readonly IDesignationService _designationService;
        private readonly IDepartmentService _departmentService;

        public DesignationController(
            IDesignationService designationService, IDepartmentService departmentService)
        {
            _designationService = designationService;
            _departmentService = departmentService;
        }

        // Display all designations
        [Authorize(Roles = "Admin,HR,Employee")]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;

            if (page < 1)
                page = 1;

            var result = await _designationService.GetPagedAsync(page, pageSize);

            int totalRecords = result.TotalRecords;

            int totalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            // Fetch again if the page number was adjusted
            if (page != 1 && page > totalPages)
            {
                result = await _designationService.GetPagedAsync(page, pageSize);
            }

            var model = new DesignationIndexViewModel
            {
                Designations = result.Designations,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            return View(model);
        }
        // Display Edit page
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var designation = await _designationService.GetByIdAsync(id);

            if (designation == null)
                return NotFound();

            var model = new DesignationEditModel
            {
                DesignationId = designation.DesignationId,
                DesignationName = designation.DesignationName,
                DepartmentId = designation.DepartmentId,
                IsActive = designation.IsActive
            };

            var departments = await _departmentService.GetAllAsync();

            ViewBag.Departments = new SelectList(
                departments,
                "Id",
                "DepartmentName",
                model.DepartmentId
            );

            return View(model);
        }

        // Save edited designation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DesignationEditModel model)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.GetAllAsync();

                ViewBag.Departments = new SelectList(
                    departments,
                    "Id",
                    "DepartmentName",
                    model.DepartmentId
                );

                return View(model);
            }

            var designation = new Designation
            {
                DesignationId = model.DesignationId,
                DesignationName = model.DesignationName,
                DepartmentId = model.DepartmentId,
                IsActive = model.IsActive
            };

            var result = await _designationService.UpdateAsync(model);

            if (!result)
            {
                ModelState.AddModelError("", "Unable to update designation.");

                var departments = await _departmentService.GetAllAsync();

                ViewBag.Departments = new SelectList(
                    departments, "Id", "DepartmentName", model.DepartmentId);

                return View(model);
            }

            TempData["SuccessMessage"] = "Designation updated successfully.";

            return RedirectToAction(nameof(Index));
        }


        //create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.GetAllAsync();

            ViewBag.Departments = new SelectList(
                departments,
                "Id",
                "DepartmentName"
            );

            return View(new DesignationcrateView());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DesignationcrateView designation)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.GetAllAsync();

                ViewBag.Departments = new SelectList(
                    departments,
                    "Id",
                    "DepartmentName"
                );

                return View(designation);
            }

            designation.CreatedDate = DateTime.Now;
            designation.UpdatedDate = null;

            try
            {
                bool result = await _designationService.CreateAsync(designation);

                if (result)
                {
                    TempData["SuccessMessage"] = "Designation created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Unable to create designation.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("DesignationName", ex.Message);
            }

            // Reload Department dropdown
            var departmentList = await _departmentService.GetAllAsync();

            ViewBag.Departments = new SelectList(
                departmentList,
                "Id",
                "DepartmentName",
                designation.DepartmentId
            );

            return View(designation);
        }

        // Delete designation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var designation =await _designationService.GetByIdAsync(id);

            if (designation == null)
            {
                return NotFound();
            }

            var result = await _designationService.DeleteAsync(id);

            if (!result)
            {
                TempData["ErrorMessage"] ="Unable to delete designation. It may be assigned to employees.";
            }
            else
            {
                TempData["SuccessMessage"] ="Designation deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetByDepartmentId(int departmentId)
        {
            var designations = await _designationService
                .GetByDepartmentIdAsync(departmentId);

            var result = designations.Select(d => new
            {
                d.DesignationId,
                d.DesignationName
            });

            return Json(result);
        }
    }
}
