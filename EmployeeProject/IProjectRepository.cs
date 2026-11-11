using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllAsync();

        Task<Project?> GetByIdAsync(int id);

        Task AddAsync(Project project);

        Task UpdateAsync(Project project);

        Task DeleteAsync(int id);

    }
}
