using EmployeeProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Data
{
    public class TaskRepository:ITaskRepository
    {
        private readonly EmployeeDbContext _context;
        public TaskRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeTask>> GetAllTasksAsync()
        {
            return await _context.EmployeeTasks
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeTask>> GetTasksByEmployeeIdAsync(int empId)
        {
            var tasks= await _context.EmployeeTasks
                .Where(t => t.EmpId == empId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
            return tasks;
        }
        public async Task<EmployeeTask?> GetTaskByIdAsync(int id)
        {
            return await _context.EmployeeTasks.FindAsync(id);
        }

        public async Task UpdateTaskAsync(EmployeeTask task)
        {
            task.UpdatedDate = DateTime.Now;

            _context.EmployeeTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.EmployeeTasks.FindAsync(id);

            if (task != null)
            {
                _context.EmployeeTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddTaskAsync(EmployeeTask task)
        {
            task.CreatedDate = DateTime.Now;

            await _context.EmployeeTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }
    }
}
