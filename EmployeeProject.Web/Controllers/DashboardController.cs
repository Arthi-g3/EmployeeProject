
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationIdentity> _userManager;

        public DashboardController(
            UserManager<ApplicationIdentity> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var roles = await _userManager.GetRolesAsync(user);

            if (user.MustChangePassword &&
                (roles.Contains("Employee") || roles.Contains("Manager")))
            {
                return RedirectToAction("ChangePassword", "Password");
            }

            ViewBag.UserRole = roles.FirstOrDefault() ?? "Employee";

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> EmployeeDashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            if (await _userManager.IsInRoleAsync(user, "Employee")
                && user.MustChangePassword)
            {
                return RedirectToAction("ChangePassword", "Password");
            }

            return View();
        }
    }
}
