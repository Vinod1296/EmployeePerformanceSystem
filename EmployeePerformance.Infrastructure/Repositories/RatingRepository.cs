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
    public class RatingRepository : IRatingRepository
    {
        private readonly EmployeePerformanceDbContext _context;

        public RatingRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            return await _context.Ratings.AsNoTracking().ToListAsync();
        }

        public async Task<Rating?> GetByIdAsync(int id)
        {
            return await _context.Ratings.AsNoTracking().FirstOrDefaultAsync(rating => rating.RatingId == id);

        }

        public async Task AddAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
        }

        public async Task <bool> DeleteAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return false;
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
