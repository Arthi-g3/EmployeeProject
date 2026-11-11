using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployeesAsync();
        Task<bool> UpdateEmployeeAsync(Employee emp);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
