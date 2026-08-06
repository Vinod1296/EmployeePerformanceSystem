using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.UserManagement;
using EmployeePerformance.Domain.Entities;
using EmployeePerformance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace EmployeePerformance.Infrastructure.Repositories
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly EmployeePerformanceDbContext _context;

        public UserManagementRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<UserListDto>> GetUsersAsync(UserQueryParametersDto parameters)
        {
            var query = BuildBaseUserQuery();

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var normalizedSearch = parameters.Search.Trim().ToUpper();
                query = query.Where(user => user.Username.ToUpper().Contains(normalizedSearch));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Role))
            {
                var normalizedRole = parameters.Role.Trim();
                query = query.Where(user => user.Role == normalizedRole);
            }

            if (parameters.IsActive.HasValue)
            {
                var isActive = parameters.IsActive.Value;
                query = query.Where(user => user.IsActive == isActive);
            }

            query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

            var totalRecords = await query.CountAsync();
            var totalPages = totalRecords == 0
                ? 0
                : (int)Math.Ceiling(totalRecords / (double)parameters.PageSize);

            var users = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(user => new UserListDto
                {
                    UserId = user.UserId,
                    EmployeeId = user.EmployeeId,
                    Username = user.Username,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                })
                .ToListAsync();

            return new PagedResponse<UserListDto>
            {
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Data = users
            };
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(int userId)
        {
            return await BuildBaseUserQuery()
                .Where(user => user.UserId == userId)
                .Select(user => new UserDetailsDto
                {
                    UserId = user.UserId,
                    EmployeeId = user.EmployeeId,
                    Username = user.Username,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserEntityByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
        }

        public async Task<User?> GetUserEntityByEmployeeIdAsync(int employeeId)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.EmployeeId == employeeId);
        }

        public async Task UpdateRoleAsync(int userId, string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user is null)
            {
                return;
            }

            user.Role = role;
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountUsersByRoleAsync(string role)
        {
            return await _context.Users.CountAsync(user => user.Role == role);
        }

        public async Task UpdateUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user is null)
            {
                return;
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountActiveAdminsAsync()
        {
            return await _context.Users.CountAsync(user => user.Role == "Admin" && user.IsActive);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private IQueryable<User> BuildBaseUserQuery()
        {
            return _context.Users.AsNoTracking().AsQueryable();
        }

        private static IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDirection)
        {
            var normalizedSortBy = sortBy.Trim();
            var isDescending = string.Equals(sortDirection.Trim(), "desc", StringComparison.OrdinalIgnoreCase);

            if (string.Equals(normalizedSortBy, "CreatedAt", StringComparison.Ordinal))
            {
                return isDescending
                    ? query.OrderByDescending(user => user.CreatedAt)
                    : query.OrderBy(user => user.CreatedAt);
            }

            return isDescending
                ? query.OrderByDescending(user => user.Username)
                : query.OrderBy(user => user.Username);
        }
    }
}
