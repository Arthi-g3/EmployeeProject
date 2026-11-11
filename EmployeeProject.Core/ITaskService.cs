using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public interface ITaskService
    {
        Task<IEnumerable<EmployeeTask>> GetAllTasksAsync();
        Task<IEnumerable<EmployeeTask>> GetTasksByEmployeeIdAsync(int empId);
        Task AddTaskAsync(EmployeeTask task);

        Task<EmployeeTask?> GetTaskByIdAsync(int id);

        Task UpdateTaskAsync(EmployeeTask task);

        Task DeleteTaskAsync(int id);
    }
}
