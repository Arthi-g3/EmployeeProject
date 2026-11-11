using EmployeeProject.Model;

namespace EmployeeProject.Core
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(int id);

        Task<bool> CreateAsync(Department department);

        Task<bool> UpdateAsync(Department department);

        Task<bool> DeleteAsync(int id);
        Task<(List<Department> Departments, int TotalRecords)>
    GetPagedAsync(int page, int pageSize);
    }
}