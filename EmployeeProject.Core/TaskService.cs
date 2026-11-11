using EmployeeProject.Data;
using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public class TaskService :ITaskService
    {
        private readonly ITaskRepository _repository;
        public TaskService(ITaskRepository repository)
        {
           _repository = repository; 
        }

        public async Task<IEnumerable<EmployeeTask>> GetAllTasksAsync()
        {
            return await _repository.GetAllTasksAsync();
        }

        public async Task<IEnumerable<EmployeeTask>> GetTasksByEmployeeIdAsync(int empId)
        {
            return await _repository.GetTasksByEmployeeIdAsync(empId);
        }
        public async Task AddTaskAsync(EmployeeTask task)
        {
            await _repository.AddTaskAsync(task);
        }

        public async Task<EmployeeTask?> GetTaskByIdAsync(int id)
        {
            return await _repository.GetTaskByIdAsync(id);
        }

        public async Task UpdateTaskAsync(EmployeeTask task)
        {
            await _repository.UpdateTaskAsync(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _repository.DeleteTaskAsync(id);
        }
    }
}
