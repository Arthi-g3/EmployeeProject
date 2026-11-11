using EmployeeProject.Model;

namespace EmployeeProject.Data.Repository
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(string departmentName, int? excludeId = null);

        Task AddAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
        Task<(List<Department> Departments, int TotalRecords)>
    GetPagedAsync(int page, int pageSize);

    }
}