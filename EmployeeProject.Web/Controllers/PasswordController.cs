
using EmployeeProject.Core;
using EmployeeProject.Core.ModelView;
using EmployeeProject.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeProject.Web.Controllers
{
    public class PasswordController : Controller
    {
        private readonly UserManager<ApplicationIdentity> _user;
        private readonly IEmailService _emailService;

        public PasswordController(
            UserManager<ApplicationIdentity> userManager,IEmailService emailService)
        {
            _user = userManager;
            _emailService = emailService;
        }

        // CHANGE PASSWORD - GET
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // CHANGE PASSWORD - POST
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            //temportarypassword and newpassword not same check

            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(
                    nameof(model.NewPassword), "New Password cannot be  the same as the temporary password");
            return View(model);
        }
            var user = await _user.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var result = await _user.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordMismatch")
                    {
                        ModelState.AddModelError(
                            nameof(model.CurrentPassword),
                            "Current password is incorrect.");
                    }
                    else if (error.Code.StartsWith("Password"))
                    {
                        ModelState.AddModelError(
                            nameof(model.NewPassword),
                            error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }
                }

                return View(model);
            }

            user.MustChangePassword = false;

            var updateResult = await _user.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            var stampResult =
                await _user.UpdateSecurityStampAsync(user);

            if (!stampResult.Succeeded)
            {
                foreach (var error in stampResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }
            string subject = "Password Changed Successfully";

            string body = $@"
    <h3>Hello,</h3>
    <p>Your WorkforceHub account password has been changed successfully.</p>
    <p>If you did not make this change, please contact your Admin or HR immediately.</p>
    <p>Regards,<br/>WorkforceHub Team</p>";

            await _emailService.SendEmailAsync(
                user.Email!,
                subject,
                body);
            TempData["Success"] =
                "Password changed successfully. Please log in with your new password.";

            await HttpContext.SignOutAsync(
                IdentityConstants.ApplicationScheme);

            return RedirectToAction("Login", "Account");
        }

        // FORGOT PASSWORD - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        // FORGOT PASSWORD - POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _user.FindByEmailAsync(model.Email);

            if (user != null && user.IsActive)
            {
                var token =
                    await _user.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action(
                    nameof(ResetPassword),
                    "Password",
                    new
                    {
                        email = model.Email,
                        token = token
                    },
                    Request.Scheme);

                // Local testing only.
                TempData["ResetLink"] = resetLink;
            }

            return RedirectToAction(
                nameof(ForgotPasswordConfirmation));
        }

        // FORGOT PASSWORD CONFIRMATION - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // RESET PASSWORD - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(
            string email,
            string token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return BadRequest();
            }

            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
        }

        // RESET PASSWORD - POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _user.FindByEmailAsync(model.Email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to reset the password.");

                return View(model);
            }

            var result = await _user.ResetPasswordAsync(
                user,
                model.Token,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;

                var updateResult = await _user.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    return View(model);
                }
            }

            TempData["Success"] =
                "Password reset successfully. Please log in.";

            return RedirectToAction("Login", "Account");
        }
    }
}
