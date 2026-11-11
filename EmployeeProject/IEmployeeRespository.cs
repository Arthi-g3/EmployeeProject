using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public interface IEmployeeRespository
    {
        Task<List<Employee>> GetEmployeesAsync();
        Task<bool> UpdateEmployeeAsync(Employee emp);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
