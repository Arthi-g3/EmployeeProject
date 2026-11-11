using EmployeeProject.Data.Repository;
using EmployeeProject.Model;

namespace EmployeeProject.Core.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(Department department)
        {
            department.DepartmentName =
                department.DepartmentName.Trim();

            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                return false;

            if (await _repository.ExistsAsync(department.DepartmentName))
                return false;

            department.CreatedDate = DateTime.Now;
            department.IsDeleted = false;

            await _repository.AddAsync(department);

            return true;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            department.DepartmentName =
                department.DepartmentName.Trim();

            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                return false;

            var existing = await _repository.GetByIdAsync(department.Id);

            if (existing == null)
                return false;

            if (await _repository.ExistsAsync(
                department.DepartmentName, department.Id))
                return false;

            existing.DepartmentName = department.DepartmentName;
            existing.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(existing);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                return false;

            await _repository.DeleteAsync(department);

            return true;
        }
        public async Task<(List<Department> Departments, int TotalRecords)>
    GetPagedAsync(int page, int pageSize)
        {
            return await _repository.GetPagedAsync(page, pageSize);
        }
    }
}