using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public class EmployeeRespository : IEmployeeRespository
    {
        private readonly EmployeeDbContext _context;
        public EmployeeRespository(EmployeeDbContext context)
        {
          _context = context;  
        }

        //public async Task<List<Employee>> GetEmployeesAsync()
        //{
        //    return await _context.Employees.ToListAsync();
        //}

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .ToListAsync();
        }

        public async Task<bool> UpdateEmployeeAsync(Employee emp)
        {
           var existing =await _context.Employees.FirstOrDefaultAsync( e=>e.EmpId==emp.EmpId);
            if (existing == null)
            {
                return false;
            }
            existing.Name = emp.Name;
            existing.Age = emp.Age;
            existing.Gender = emp.Gender;
            existing.Email = emp.Email;
            existing.Department = emp.Department;
            existing.Designation = emp.Designation;
            existing.PhoneNumber = emp.PhoneNumber;
            existing.City = emp.City;
            existing.Salary = emp.Salary;

            existing.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        //Delete

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
           var emp= await _context.Employees.FirstOrDefaultAsync(e=>e.EmpId == id);
            if(emp == null)
            {
                return false;
            }
            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
