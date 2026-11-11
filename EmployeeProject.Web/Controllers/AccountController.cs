
using EmployeeProject.Core.ModelView;
using EmployeeProject.Data;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationIdentity> _user;
        private readonly SignInManager<ApplicationIdentity> _sign;

        public AccountController(
            UserManager<ApplicationIdentity> user,
            SignInManager<ApplicationIdentity> sign)
        {
            _user = user;
            _sign = sign;
        }

        // Login - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login - POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _user.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is inactive. Contact Admin or HR.");

                return View(model);
            }

            var result = await _sign.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            bool isAdmin = await _user.IsInRoleAsync(user, "Admin");
            bool isHR = await _user.IsInRoleAsync(user, "HR");
            bool isManager = await _user.IsInRoleAsync(user, "Manager");
            bool isEmployee = await _user.IsInRoleAsync(user, "Employee");

            if (!isAdmin && !isHR && !isManager && !isEmployee)
            {
                await _sign.SignOutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "No valid role is assigned to this account.");

                return View(model);
            }

            // Check EmployeeId for all roles
            if (user.EmployeeId == null)
            {
                await _sign.SignOutAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Employee ID is not linked to your account.");

                return View(model);
            }

            // Employee must change temporary password
            if ((isEmployee || isManager) && user.MustChangePassword)
            {
                return RedirectToAction(
                    "ChangePassword",
                    "Password");
            }

            // Add EmpId claim for Admin, HR,Manager and Employee
            var claims = new List<Claim>
            {
                new Claim(
                    "EmpId",
                    user.EmployeeId.Value.ToString())
            };

            await _sign.SignInWithClaimsAsync(
                user,
                model.RememberMe,
                claims);

            return RedirectToAction("Index", "Dashboard");
        }

        // Register - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Public registration is disabled.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(EmployeeRegisterView model)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                "Public registration is disabled. Contact Admin or HR.");
        }

        // Logout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _sign.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        // Access Denied
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
