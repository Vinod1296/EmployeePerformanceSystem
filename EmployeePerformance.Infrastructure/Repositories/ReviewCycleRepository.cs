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
    public class ReviewCycleRepository : IReviewCycleRepository
    {
        private readonly EmployeePerformanceDbContext _context;

        public ReviewCycleRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewCycle>> GetAllAsync()
        {
            return await _context.ReviewCycles.AsNoTracking().ToListAsync();
        }

        public async Task<ReviewCycle?> GetByIdAsync(int id)
        {
            return await _context.ReviewCycles.AsNoTracking().FirstOrDefaultAsync(reviewCycle => reviewCycle.ReviewCycleId == id);
        }

        public async Task AddAsync(ReviewCycle reviewCycle)
        {
            await _context.ReviewCycles.AddAsync(reviewCycle);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(ReviewCycle reviewCycle)
        {
            _context.ReviewCycles.Update(reviewCycle);
            await _context.SaveChangesAsync();
        }


        public async Task <bool>DeleteAsync(int id)
        {
            var reviewCycle = await _context.ReviewCycles.FindAsync(id);

            if (reviewCycle == null)
                return false;

            _context.ReviewCycles.Remove(reviewCycle);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
