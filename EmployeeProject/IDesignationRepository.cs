using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public interface IDesignationRepository
    {
        Task<List<Designation>> GetAllAsync();

        Task<Designation?> GetByIdAsync(int id);

        Task<bool> CreateAsync(Designation designation);

        Task<bool> UpdateAsync(Designation designation);

        Task<bool> DeleteAsync(int id);
        Task<List<Designation>> GetByDepartmentIdAsync(int departmentId);
        Task<(List<Designation> Designations, int TotalRecords)>
    GetPagedAsync(int page, int pageSize);
    }
}

