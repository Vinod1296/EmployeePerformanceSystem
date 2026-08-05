using EmployeePerformance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);

        Task<User?> GetUserByIdAsync(int userId);

        Task<bool> UsernameExistsAsync(string username);

        Task<bool> EmployeeExistsAsync(int employeeId);

        Task<Employee?> GetEmployeeByIdAsync(int employeeId);

        Task<bool> EmployeeAlreadyRegisteredAsync(int employeeId);

        Task<User> CreateUserAsync(User user);

        Task ChangePasswordAsync(int userId, string newPasswordHash);
    }
}
