using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;
using EmployeePerformance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace EmployeePerformance.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeePerformanceDbContext _context;

        public EmployeeRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.AsNoTracking().ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(employee => employee.EmployeeId == id);
        }


        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

      public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Employee>> SearchEmployeesAsync(EmployeeSearchDto searchDto)

        {
            var query = _context.Employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.FirstName))
                {
                query = query.Where(e => e.FirstName.Contains(searchDto.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.LastName))
            {
                query = query.Where(e => e.LastName.Contains(searchDto.LastName));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Department))
            {
                query = query.Where(e => e.Department == searchDto.Department);
            }

            //sorting

            if (!string.IsNullOrWhiteSpace(searchDto.SortBy))
            {
                switch (searchDto.SortBy.ToLower())
                {

                    case "firstname":
                        query = searchDto.SortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.FirstName)
                            : query.OrderBy(e => e.FirstName);
                        break;

                        case "lastname":
                        query = searchDto.SortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.LastName)
                            : query.OrderBy(e => e.LastName);
                        break;

                    case "department":
                        query = searchDto.SortDirection?.ToLower() == "desc"
                            ? query.OrderByDescending(e => e.Department)
                            : query.OrderBy(e => e.Department);
                        break;

                        default: 

                        query = query.OrderBy(e => e.EmployeeId);
                        break;
                }
            }

            // pagination

            query = query 
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize);
            return await query.ToListAsync();
        }
    }
}
