using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Domain.Entities;
namespace EmployeePerformance.Application.Services
{
    public  class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserManagementRepository _userManagementRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserManagementRepository userManagementRepository)
        {
            _employeeRepository = employeeRepository;
            _userManagementRepository = userManagementRepository;
        }

        public async Task<EmployeeDto> AddEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            var employee = new Employee();

            employee.EmployeeCode = employeeDto.EmployeeCode;
            employee.FirstName = employeeDto.FristName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.Department = employeeDto.Department;
            employee.Designation = employeeDto.Designation;
            employee.ManagerId = employeeDto.ManagerId;
            employee.HireDate = employeeDto.HireDate;

            await ValidateManagerAsync(employee.ManagerId, null);

            employee.IsActive = true;
            employee.CreatedAt = DateTime.UtcNow;
            employee.ModifiedAt = DateTime.UtcNow;

            await _employeeRepository.AddAsync(employee);

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FristName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Department = employee.Department ?? string.Empty,
                Designation = employee.Designation ?? string.Empty,
                ManagerId = employee.ManagerId,
                HireDate = employee.HireDate.HasValue
                    ? employee.HireDate.Value.ToDateTime(TimeOnly.MinValue)
                    : DateTime.MinValue,
                IsActive = employee.IsActive
            };
        }

        //public async Task DeleteEmployeeAsync(int id)
        //{
        //    var employee = await _employeeRepository.GetByIdAsync(id);

        //    if (employee == null)
        //    {
        //        return;
        //    }

        //    await _employeeRepository.DeleteAsync(employee);
        //}


        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return false; // Employee not found
            }
            await _employeeRepository.DeleteAsync(employee);
            return true; // Employee deleted successfully
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return null;
            }
            var employeeDto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FristName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Department = employee.Department ?? string.Empty,
                Designation = employee.Designation ?? string.Empty,
                ManagerId = employee.ManagerId,
                HireDate = employee.HireDate.HasValue ? new DateTime(employee.HireDate.Value.Year, employee.HireDate.Value.Month, employee.HireDate.Value.Day) : DateTime.MinValue,
                IsActive = employee.IsActive
            };

            return employeeDto;
        }

        public  async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
           var employees = await _employeeRepository.GetAllAsync();
            
            var employeeDtos = new List<EmployeeDto>();

 
                foreach (var employee in employees)
                {
                    var employeeDto = new EmployeeDto
                    {
                        EmployeeId = employee.EmployeeId,
                        EmployeeCode = employee.EmployeeCode,
                        FristName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        Department = employee.Department ?? string.Empty,
                        Designation = employee.Designation ?? string.Empty,
                        ManagerId = employee.ManagerId,
                        HireDate = employee.HireDate.HasValue ? new DateTime(employee.HireDate.Value.Year, employee.HireDate.Value.Month, employee.HireDate.Value.Day) : DateTime.MinValue,
                        IsActive = employee.IsActive
                    };
                employeeDtos.Add(employeeDto);
                }
                return employeeDtos;
            }

        public async  Task UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto)
        {
            
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return;
            }
            await ValidateManagerAsync(employeeDto.ManagerId, id);
            employee.FirstName = employeeDto.FristName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.Department = employeeDto.Department;
            employee.Designation = employeeDto.designation;
            employee.ManagerId = employeeDto.ManagerId;
            employee.HireDate = employeeDto.HireDate;
            employee.ModifiedAt = DateTime.UtcNow;
            employee.IsActive = employeeDto.IsActive;
            await _employeeRepository.UpdateAsync(employee);
        }

        private async Task ValidateManagerAsync(int? managerId, int? employeeId)
        {
            if (!managerId.HasValue)
            {
                return;
            }

            if (employeeId.HasValue && employeeId.Value == managerId.Value)
            {
                throw new InvalidOperationException("An employee cannot be their own manager.");
            }

            var managerEmployee = await _employeeRepository.GetByIdAsync(managerId.Value);
            if (managerEmployee is null)
            {
                throw new InvalidOperationException("Selected manager not found.");
            }

            if (!managerEmployee.IsActive)
            {
                throw new InvalidOperationException("Selected manager is inactive.");
            }

            var managerUser = await _userManagementRepository.GetUserEntityByEmployeeIdAsync(managerId.Value);
            if (managerUser is null)
            {
                throw new InvalidOperationException("Selected manager user account not found.");
            }

            if (!managerUser.IsActive)
            {
                throw new InvalidOperationException("Selected manager user account is inactive.");
            }

            if (!string.Equals(managerUser.Role, "Manager", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Selected employee is not a Manager.");
            }
        }


        public async Task<List<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchDto searchDto)
        {
            var employees = await _employeeRepository.SearchEmployeesAsync(searchDto);

            return employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                EmployeeCode = e.EmployeeCode,
                FristName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Department = e.Department ?? string.Empty,
                Designation = e.Designation ?? string.Empty,
                ManagerId = e.ManagerId,
                HireDate = e.HireDate.HasValue ? new DateTime(e.HireDate.Value.Year, e.HireDate.Value.Month, e.HireDate.Value.Day) : DateTime.MinValue,
                IsActive = e.IsActive
            }).ToList();

        }
           
    }
}
