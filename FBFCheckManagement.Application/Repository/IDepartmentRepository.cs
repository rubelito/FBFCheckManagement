using System.Collections.Generic;
using FBFCheckManagement.Application.Domain;

namespace FBFCheckManagement.Application.Repository
{
    public interface IDepartmentRepository
    {
        void AddDepartment(Department department);
        List<Department> GetAllDepartments();
        Department GetDepartmentById(long id);
        void EditDepartment(Department dToEdit);
        bool IsSuccess { get; }
        string StatusMessage { get; }
    }
}