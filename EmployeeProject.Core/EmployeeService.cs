using EmployeeProject.Data;
using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRespository _repository;
        public EmployeeService(IEmployeeRespository respository)
        {
            _repository = respository;
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _repository.GetEmployeesAsync();
        }

        public async Task<bool> UpdateEmployeeAsync(Employee emp)
        {
            return await _repository.UpdateEmployeeAsync(emp);
        }
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
           return await _repository.DeleteEmployeeAsync(id);
        }
    }
}
