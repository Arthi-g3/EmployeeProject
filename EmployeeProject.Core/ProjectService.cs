using EmployeeProject.Data;
using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<List<Project>> GetAllAsync()
        {
            return await _projectRepository.GetAllAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Project project)
        {
            await _projectRepository.AddAsync(project);
        }

        public async Task UpdateAsync(Project project)
        {
            await _projectRepository.UpdateAsync(project);
        }

        public async Task DeleteAsync(int id)
        {
            await _projectRepository.DeleteAsync(id);
        }

    }
}




    
