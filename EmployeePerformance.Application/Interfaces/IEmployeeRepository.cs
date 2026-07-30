using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Interfaces
{
    public  interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);
        Task<List<Employee>> SearchEmployeesAsync(EmployeeSearchDto searchDto);


    }
}
