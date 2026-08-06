using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.UserManagement;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IUserManagementRepository
    {
        Task<PagedResponse<UserListDto>> GetUsersAsync(UserQueryParametersDto parameters);

        Task<UserDetailsDto?> GetUserByIdAsync(int userId);

        Task<User?> GetUserEntityByIdAsync(int userId);

        Task<User?> GetUserEntityByEmployeeIdAsync(int employeeId);

        Task UpdateRoleAsync(int userId, string role);

        Task<int> CountUsersByRoleAsync(string role);

        Task UpdateUserStatusAsync(int userId, bool isActive);

        Task<int> CountActiveAdminsAsync();

        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
