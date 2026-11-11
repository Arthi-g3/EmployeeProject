using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly EmployeeDbContext _context;
        public ProjectRepository(EmployeeDbContext context)
        {
            _context = context;
        }
        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects
                .OrderByDescending(p => p.ProjectId)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p=>p.EmployeeTasks)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

    }
}

    
