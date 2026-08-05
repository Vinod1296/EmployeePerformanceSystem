using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeePerformance.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly EmployeePerformanceDbContext _context;

        public DashboardRepository(EmployeePerformanceDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetActiveEmployeeCountAsync()
        {
            return await _context.Employees.CountAsync(employee => employee.IsActive);
        }

        public async Task<int> GetPendingReviewCountAsync()
        {
            return await _context.PerformanceReviews.CountAsync(performanceReview =>
                performanceReview.Status == "Draft" ||
                performanceReview.Status == "Submitted" ||
                performanceReview.Status == "NeedsRevision");
        }

        public async Task<int> GetCompletedReviewCountAsync()
        {
            return await _context.PerformanceReviews.CountAsync(performanceReview => performanceReview.Status == "Approved");
        }

        public async Task<decimal?> GetAverageApprovedRatingAsync()
        {
            var approvedRatings = _context.PerformanceReviews
                .Where(performanceReview => performanceReview.Status == "Approved" && performanceReview.OverallRating != null)
                .Select(performanceReview => performanceReview.OverallRating!.Value);

            if (!await approvedRatings.AnyAsync())
            {
                return null;
            }

            return await approvedRatings.AverageAsync();
        }
    }
}
