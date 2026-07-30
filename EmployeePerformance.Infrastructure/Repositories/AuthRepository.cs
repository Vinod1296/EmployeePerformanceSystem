using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;
using EmployeePerformance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EmployeePerformanceDbContext _context;

        public AuthRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var normalizedUsername = NormalizeUsername(username);

            return await _context.Users.AsNoTracking()
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == normalizedUsername);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(user => user.UserId == userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            var normalizedUsername = NormalizeUsername(username);

            return await _context.Users.AnyAsync(user => user.Username == normalizedUsername);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees.AsNoTracking().AnyAsync(employee => employee.EmployeeId == employeeId);
        }

        public async Task<bool> EmployeeAlreadyRegisteredAsync(int employeeId)
        {
            return await _context.Users.AsNoTracking().AnyAsync(user => user.EmployeeId == employeeId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task ChangePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user is null)
            {
                return;
            }

            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
        }

        private static string NormalizeUsername(string username)
        {
            return username.Trim().ToUpperInvariant();
        }
    }
}
