using EmployeeProject.Data;
using EmployeeProject.Model;

namespace EmployeeProject.Core
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _repository;

        public TimesheetService(ITimesheetRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddAsync(Timesheet timesheet)
        {
            if (timesheet.Hours < 1 || timesheet.Hours > 8)
            {
                return false;
            }

            timesheet.WorkDate = timesheet.WorkDate.Date;

            if (await _repository.ExistsForEmployeeAndDateAsync(
                    timesheet.EmployeeId,
                    timesheet.WorkDate))
            {
                return false;
            }

            timesheet.SubmittedDate = DateTime.Now;
            await _repository.AddAsync(timesheet);
            return true;
        }

        public async Task<List<Timesheet>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _repository.GetByEmployeeIdAsync(employeeId);
        }

        public async Task<List<Timesheet>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> ExistsForEmployeeAndDateAsync(
            int employeeId,
            DateTime workDate)
        {
            return await _repository.ExistsForEmployeeAndDateAsync(employeeId, workDate);
        }
    }
}
