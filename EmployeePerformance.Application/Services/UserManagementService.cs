using EmployeePerformance.Application.DTOs.UserManagement;
using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private static readonly HashSet<string> SupportedRoles = new(StringComparer.Ordinal)
        {
            "Admin",
            "Manager",
            "Employee"
        };

        private readonly IUserManagementRepository _userManagementRepository;

        public UserManagementService(IUserManagementRepository userManagementRepository)
        {
            _userManagementRepository = userManagementRepository;
        }

        public async Task<PagedResponse<UserListDto>> GetUsersAsync(UserQueryParametersDto parameters)
        {
            ValidateQueryParameters(parameters);
            return await _userManagementRepository.GetUsersAsync(parameters);
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(int userId)
        {
            return await _userManagementRepository.GetUserByIdAsync(userId);
        }

        public async Task<UpdateUserRoleResponseDto> UpdateUserRoleAsync(int userId, string role, int authenticatedUserId)
        {
            role = Normalize(role);
            ValidateRole(role);

            if (userId == authenticatedUserId)
            {
                throw new CannotChangeOwnRoleException();
            }

            var user = await _userManagementRepository.GetUserEntityByIdAsync(userId);
            if (user is null)
            {
                throw new UserNotFoundException();
            }

            if (string.Equals(user.Role, role, StringComparison.Ordinal))
            {
                throw new SameRoleException();
            }

            if (string.Equals(user.Role, "Admin", StringComparison.Ordinal) &&
                !string.Equals(role, "Admin", StringComparison.Ordinal))
            {
                var adminCount = await _userManagementRepository.CountUsersByRoleAsync("Admin");
                if (adminCount <= 1)
                {
                    throw new CannotRemoveLastAdminException();
                }
            }

            var oldRole = user.Role;
            await _userManagementRepository.UpdateRoleAsync(user.UserId, role);

            return new UpdateUserRoleResponseDto
            {
                Message = "User role updated successfully.",
                UserId = user.UserId,
                Username = user.Username,
                OldRole = oldRole,
                NewRole = role
            };
        }

        public async Task<UpdateUserStatusResponseDto> UpdateUserStatusAsync(int userId, bool isActive, int authenticatedUserId)
        {
            var user = await _userManagementRepository.GetUserEntityByIdAsync(userId);
            if (user is null)
            {
                throw new UserNotFoundException();
            }

            var oldStatus = user.IsActive;

            if (user.IsActive == isActive)
            {
                throw new UserStatusAlreadyUpdatedException();
            }

            if (!isActive && user.UserId == authenticatedUserId)
            {
                throw new CannotDeactivateOwnAccountException();
            }

            if (string.Equals(user.Role, "Admin", StringComparison.Ordinal) && !isActive)
            {
                await _userManagementRepository.ExecuteInTransactionAsync(async () =>
                {
                    var activeAdminCount = await _userManagementRepository.CountActiveAdminsAsync();
                    if (activeAdminCount <= 1)
                    {
                        throw new CannotDeactivateLastActiveAdminException();
                    }

                    await _userManagementRepository.UpdateUserStatusAsync(user.UserId, isActive);
                });
            }
            else
            {
                await _userManagementRepository.UpdateUserStatusAsync(user.UserId, isActive);
            }

            return new UpdateUserStatusResponseDto
            {
                Message = "User status updated successfully.",
                UserId = user.UserId,
                Username = user.Username,
                OldStatus = oldStatus,
                NewStatus = isActive
            };
        }

        private static void ValidateRole(string role)
        {
            if (!SupportedRoles.Contains(role))
            {
                throw new InvalidRoleException();
            }
        }

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static void ValidateQueryParameters(UserQueryParametersDto parameters)
        {
            if (parameters.PageNumber <= 0)
            {
                throw new InvalidPageNumberException();
            }

            if (parameters.PageSize <= 0)
            {
                throw new InvalidPageSizeException();
            }

            if (parameters.PageSize > 100)
            {
                throw new PageSizeExceededException();
            }

            var sortBy = Normalize(parameters.SortBy);
            if (!string.Equals(sortBy, "Username", StringComparison.Ordinal) &&
                !string.Equals(sortBy, "CreatedAt", StringComparison.Ordinal))
            {
                throw new InvalidSortByException();
            }

            var sortDirection = Normalize(parameters.SortDirection);
            if (!string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidSortDirectionException();
            }
        }
    }
}
