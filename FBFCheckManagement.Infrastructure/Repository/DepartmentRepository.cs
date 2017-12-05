using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Infrastructure.EntityFramework;

namespace FBFCheckManagement.Infrastructure.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly FBFDbContext _context;
        private string _statusMessage = string.Empty;
        private bool _isSuccess = false;

        public DepartmentRepository(IDatabaseType databaseType)
        {
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new FBFDbContext(databaseType);
        }

        public void AddDepartment(Department department)
        {
            bool isExist = _context.Departments.Any(d => d.Name == department.Name);

            if (isExist != true)
            {
                _context.Departments.Add(department);
                _context.SaveChanges();
                _isSuccess = true;
                _statusMessage = "success adding department";
            }
            else
            {
                _isSuccess = false;
                _statusMessage = "failed adding department: department already exist";
            }
        }

        public List<Department> GetAllDepartments()
        {
            return _context.Departments.ToList();
        }

        public Department GetDepartmentById(long id)
        {
            return _context.Departments.FirstOrDefault(d => d.Id == id);
        }

        public void EditDepartment(Department dToEdit)
        {
            bool isExist = _context.Departments.Any(d => d.Name == dToEdit.Name);

            if (isExist == false)
            {
                Department oldDepartment = _context.Departments.FirstOrDefault(d => d.Id == dToEdit.Id);

                oldDepartment.Name = dToEdit.Name;
                oldDepartment.ModifiedDate = DateTime.Now;

                _context.SaveChanges();
                _isSuccess = true;
                _statusMessage = "success editing Bank";
            }
            else
            {
                _isSuccess = false;
                _statusMessage = "failed editing bank: bank already exist";
            }
        }

        public bool IsSuccess { get { return _isSuccess; } }
        public string StatusMessage { get { return _statusMessage; } }
    }
}