using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public  interface IEmployeeService
    {
        // IEmployeeService
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();

        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

        Task<EmployeeDto> AddEmployeeAsync(CreateEmployeeDto employeeDto);

        Task UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<List<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchDto searchDto);
    }

}
