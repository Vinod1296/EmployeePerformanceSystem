using EmployeePerformance.Application.DTOs.UserManagement;
using EmployeePerformance.Application.DTOs.Common;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<PagedResultDto<UserListDto>> GetUsersAsync(UserQueryParametersDto parameters);

        Task<UserDetailsDto?> GetUserByIdAsync(int userId);

        Task<UpdateUserRoleResponseDto> UpdateUserRoleAsync(int userId, string role, int authenticatedUserId);

        Task<UpdateUserStatusResponseDto> UpdateUserStatusAsync(int userId, bool isActive, int authenticatedUserId);
    }
}
