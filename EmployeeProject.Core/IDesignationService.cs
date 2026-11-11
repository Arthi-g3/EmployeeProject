using EmployeeProject.Core.ModelView;
using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public interface IDesignationService
    {
        Task<List<Designation>> GetAllAsync();

        Task<Designation?> GetByIdAsync(int id);

        Task<bool> CreateAsync(DesignationcrateView designation);

        Task<bool> UpdateAsync(DesignationEditModel designation);

        Task<bool> DeleteAsync(int id);
        Task<List<Designation>> GetByDepartmentIdAsync(int departmentId);
        Task<(List<Designation> Designations, int TotalRecords)>
    GetPagedAsync(int page, int pageSize);
    }
}
