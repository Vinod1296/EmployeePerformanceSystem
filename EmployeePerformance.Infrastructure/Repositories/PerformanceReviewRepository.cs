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
    public class PerformanceReviewRepository : IPerformanceReviewRepository
    {

        private readonly EmployeePerformanceDbContext _context;

        public PerformanceReviewRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PerformanceReview>> GetAllAsync()
        {
            return await _context.PerformanceReviews.AsNoTracking().ToListAsync();

        }

        public async Task<IEnumerable<PerformanceReview>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.PerformanceReviews
                .AsNoTracking()
                .Where(performanceReview => performanceReview.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<PerformanceReview?> GetByIdAsync(int id)

        {
            return await _context.PerformanceReviews.AsNoTracking().FirstOrDefaultAsync(performanceReview => performanceReview.PerformanceReviewId == id);
        }

        public async Task<PerformanceReview?> GetPerformanceReviewByIdAsync(int id)
        {
            return await _context.PerformanceReviews
                .Include(performanceReview => performanceReview.ReviewCycle)
                .FirstOrDefaultAsync(performanceReview => performanceReview.PerformanceReviewId == id);
        }

        public async Task<PerformanceReview?> GetByIdForEmployeeAsync(int id, int employeeId)
        {
            return await _context.PerformanceReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(performanceReview =>
                    performanceReview.PerformanceReviewId == id &&
                    performanceReview.EmployeeId == employeeId);
        }

        public async Task<bool> ExistsByEmployeeAndCycleAsync(int employeeId, int reviewCycleId)
        {
            return await _context.PerformanceReviews.AnyAsync(performanceReview =>
                performanceReview.EmployeeId == employeeId &&
                performanceReview.ReviewCycleId == reviewCycleId);
        }

        public async Task AddAsync(PerformanceReview performanceReview)
        {
            await _context.PerformanceReviews.AddAsync(performanceReview);
            await _context.SaveChangesAsync();
        }

        public async Task SubmitSelfAssessmentAsync(PerformanceReview performanceReview)
        {
            _context.Entry(performanceReview).Property(x => x.SelfAssessment).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.SubmittedDate).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.Status).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.ModifiedAt).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateManagerReviewAsync(PerformanceReview performanceReview)
        {
            _context.Entry(performanceReview).Property(x => x.ManagerComments).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.OverallRating).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.Status).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.ApprovedDate).IsModified = true;
            _context.Entry(performanceReview).Property(x => x.ModifiedAt).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var performanceReview = await _context.PerformanceReviews.FindAsync(id);

                if (performanceReview == null)
                return false; ;
            _context.PerformanceReviews.Remove(performanceReview);

            await _context.SaveChangesAsync();

            return true;

          
        }
    }
}
