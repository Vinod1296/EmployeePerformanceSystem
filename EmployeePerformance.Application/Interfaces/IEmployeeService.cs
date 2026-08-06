using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();

        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);

        Task<EmployeeDto> AddEmployeeAsync(CreateEmployeeDto employeeDto);

        Task UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<PagedResponse<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchDto searchDto);
    }
}
