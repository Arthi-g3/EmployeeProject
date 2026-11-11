using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Data.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly EmployeeDbContext _context;

        public DepartmentRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Where(d => !d.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d =>
                    d.Id == id && !d.IsDeleted);
        }

        public async Task<bool> ExistsAsync(
            string departmentName, int? excludeId = null)
        {
            return await _context.Departments.AnyAsync(d =>
                !d.IsDeleted &&
                d.DepartmentName == departmentName &&
                (!excludeId.HasValue || d.Id != excludeId.Value));
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            department.IsDeleted = true;
            department.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }
        public async Task<(List<Department> Departments, int TotalRecords)>
    GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Departments
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            int totalRecords = await query.CountAsync();

            var departments = await query
                .OrderBy(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (departments, totalRecords);
        }
    }
}