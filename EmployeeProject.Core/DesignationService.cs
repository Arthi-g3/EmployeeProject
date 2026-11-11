using EmployeeProject.Core.ModelView;
using EmployeeProject.Data;
using EmployeeProject.Data.Repository;
using EmployeeProject.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeProject.Core
{
    public  class DesignationService:IDesignationService
    {
        private readonly IDesignationRepository _repository;
        public DesignationService(IDesignationRepository repository)
        {
           _repository = repository; 
        }
        public Task<List<Designation>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Designation?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(DesignationcrateView designation)
        {
            string designationName = designation.DesignationName.Trim();

            var designations = await _repository.GetAllAsync();

            bool exists = designations.Any(d =>
                d.DesignationName.Trim().Equals(
                    designationName,
                    StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                throw new Exception("Designation name already exists.");
            }

            var desi = new Designation
            {
                DepartmentId = designation.DepartmentId,
                DesignationName = designationName,
                IsActive = designation.IsActive,
                CreatedDate = designation.CreatedDate,
                UpdatedDate = null
            };

            return await _repository.CreateAsync(desi);
        }

        public Task<bool> UpdateAsync(DesignationEditModel designation)
        {
            var desi = new Designation
            {
                DepartmentId= designation.DepartmentId,
                DesignationName= designation.DesignationName,
                DesignationId= designation.DesignationId,
                IsActive= designation.IsActive,
            };
            return _repository.UpdateAsync(desi);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
        public async Task<List<Designation>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _repository.GetByDepartmentIdAsync(departmentId);
        }
        public async Task<(List<Designation> Designations, int TotalRecords)>
    GetPagedAsync(int page, int pageSize)
        {
            return await _repository.GetPagedAsync(page, pageSize);
        }
    }
}
