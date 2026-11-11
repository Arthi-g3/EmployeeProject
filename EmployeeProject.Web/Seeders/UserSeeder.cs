
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using EmployeeProject.Model;
using EmployeeProject.Data;
using EmployeeProject.Model.Enums;

namespace EmployeeProject.Web.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(
            IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationIdentity>>();

            var context = serviceProvider
                .GetRequiredService<EmployeeDbContext>();

            var users = new[]
            //Hr@12345
            {
                (
                    Email: "Admin@12345",
                    Password: "Admin@12345",
                    Role: "Admin",
                    DepartmentId: 1,
                    DesignationId: 11,
                    Name: "Admin",
                    Gender: Gender.Male,
                    Phone: "9000000001"
                ),
                (
                    Email: "hr@gmail.com",
                    Password: "Hr@12345",
                    Role: "HR",
                    DepartmentId: 2,
                    DesignationId: 4,
                    Name: "HR",
                    Gender: Gender.Female,
                    Phone: "9000000002"
                )
            };

            foreach (var item in users)
            {
                var user = await userManager.FindByEmailAsync(item.Email);

                if (user == null)
                {
                    user = new ApplicationIdentity
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(
                        user, item.Password);

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not create {item.Role}: " +
                            string.Join("; ",
                                result.Errors.Select(e => e.Description)));
                    }
                }

                if (!await userManager.IsInRoleAsync(user, item.Role))
                {
                    var roleResult = await userManager.AddToRoleAsync(
                        user, item.Role);

                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not assign {item.Role} role: " +
                            string.Join("; ",
                                roleResult.Errors.Select(e => e.Description)));
                    }
                }

                // Find existing Employee record
                var employee = await context.Employees
                    .FirstOrDefaultAsync(e => e.Email == item.Email);

                // Create Employee record if it does not exist
                if (employee == null)
                {
                    employee = new Employee
                    {
                        Name = item.Name,
                        Age = 30,
                        Gender = item.Gender,
                        Email = item.Email,
                        DepartmentId = item.DepartmentId,
                        DesignationId = item.DesignationId,
                        PhoneNumber = item.Phone,
                        City = "Chennai",
                        Salary = 30000,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    context.Employees.Add(employee);
                    await context.SaveChangesAsync();
                }

                // Link Identity account to Employee record
                if (user.EmployeeId != employee.EmpId)
                {
                    user.EmployeeId = employee.EmpId;
                    user.UpdatedDate = DateTime.Now;

                    var updateResult = await userManager.UpdateAsync(user);

                    if (!updateResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not link Employee ID for {item.Role}: " +
                            string.Join("; ",
                                updateResult.Errors.Select(e => e.Description)));
                    }
                }

                // Assign Manager role to existing employee account
                var managerEmail = "arthigopalmanager@gmail.com";

                var managerUser = await userManager.FindByEmailAsync(managerEmail);

                if (managerUser != null)
                {
                    // Remove Employee role
                    if (await userManager.IsInRoleAsync(managerUser, "Employee"))
                    {
                        var removeResult = await userManager.RemoveFromRoleAsync(
                            managerUser, "Employee");

                        if (!removeResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                string.Join("; ",
                                    removeResult.Errors.Select(e => e.Description)));
                        }
                    }

                    // Add Manager role
                    if (!await userManager.IsInRoleAsync(managerUser, "Manager"))
                    {
                        var addResult = await userManager.AddToRoleAsync(
                            managerUser, "Manager");

                        if (!addResult.Succeeded)
                        {
                            throw new InvalidOperationException(
                                string.Join("; ",
                                    addResult.Errors.Select(e => e.Description)));
                        }
                    }
                }

            }
        }
    }
}
