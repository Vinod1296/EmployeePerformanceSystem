using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);

        Task<PagedResponse<Employee>> SearchEmployeesAsync(EmployeeSearchDto searchDto);
    }
}
