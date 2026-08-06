using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.Employee;
using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class EmployeeService : IEmployeeService
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
            var employee = new Employee
            {
                EmployeeCode = employeeDto.EmployeeCode,
                FirstName = employeeDto.FristName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Department = employeeDto.Department,
                Designation = employeeDto.Designation,
                ManagerId = employeeDto.ManagerId,
                HireDate = employeeDto.HireDate
            };

            await ValidateManagerAsync(employee.ManagerId, null);

            employee.IsActive = true;
            employee.CreatedAt = DateTime.UtcNow;
            employee.ModifiedAt = DateTime.UtcNow;

            await _employeeRepository.AddAsync(employee);

            return MapToDto(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return false;
            }

            await _employeeRepository.DeleteAsync(employee);
            return true;
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee is null ? null : MapToDto(employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(MapToDto).ToList();
        }

        public async Task UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto)
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

        public async Task<PagedResponse<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchDto searchDto)
        {
            searchDto.PageNumber = searchDto.PageNumber <= 0 ? 1 : searchDto.PageNumber;
            searchDto.PageSize = searchDto.PageSize <= 0 ? 10 : searchDto.PageSize;

            var pagedEmployees = await _employeeRepository.SearchEmployeesAsync(searchDto);

            return new PagedResponse<EmployeeDto>
            {
                PageNumber = pagedEmployees.PageNumber,
                PageSize = pagedEmployees.PageSize,
                TotalRecords = pagedEmployees.TotalRecords,
                TotalPages = pagedEmployees.TotalPages,
                Data = pagedEmployees.Data.Select(MapToDto).ToList()
            };
        }

        private static EmployeeDto MapToDto(Employee employee)
        {
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
                    ? new DateTime(employee.HireDate.Value.Year, employee.HireDate.Value.Month, employee.HireDate.Value.Day)
                    : DateTime.MinValue,
                IsActive = employee.IsActive
            };
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
    }
}
