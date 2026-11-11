

using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Data
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly EmployeeDbContext _context;

        public DesignationRepository(EmployeeDbContext context)
        {
            _context = context;
        }
        //Get All
        public async Task<List<Designation>> GetAllAsync()
        {
            return await _context.Designations
                .OrderBy(d => d.DesignationName)
                .ToListAsync();
        }
        //GEtByID
        public async Task<Designation?> GetByIdAsync(int id)
        {
            return await _context.Designations
                .FirstOrDefaultAsync(d => d.DesignationId == id);
        }
        //Create
        public async Task<bool> CreateAsync(Designation designation)
        {
            designation.CreatedDate = DateTime.Now;

            _context.Designations.Add(designation);

            return await _context.SaveChangesAsync() > 0;
        }
        // Update
        public async Task<bool> UpdateAsync(Designation designation)
        {
            var existing = await _context.Designations
                .FirstOrDefaultAsync(d =>
                    d.DesignationId == designation.DesignationId);

            if (existing == null)
                return false;
            existing.DepartmentId = designation.DepartmentId;
            existing.DesignationName = designation.DesignationName;
            existing.IsActive = designation.IsActive;
            existing.UpdatedDate = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }
        //Delete
        public async Task<bool> DeleteAsync(int id)
        {
            var designation = await _context.Designations
                .FirstOrDefaultAsync(d => d.DesignationId == id);

            if (designation == null)
                return false;

            bool hasEmployees = await _context.Employees
                .AnyAsync(e => e.DesignationId == id);

            if (hasEmployees)
                return false;

            _context.Designations.Remove(designation);

            return await _context.SaveChangesAsync() > 0;
        }
        //getByDepartmentID
        public async Task<List<Designation>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Designations
                .Where(d => d.DepartmentId == departmentId && d.IsActive)
                .OrderBy(d => d.DesignationName)
                .ToListAsync();
        }
        //pagination
        public async Task<(List<Designation> Designations, int TotalRecords)>
    GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Designations
                .AsNoTracking();
              

            int totalRecords = await query.CountAsync();

            var designations = await query
                .OrderBy(d => d.DesignationId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (designations, totalRecords);
        }
    }
    }
