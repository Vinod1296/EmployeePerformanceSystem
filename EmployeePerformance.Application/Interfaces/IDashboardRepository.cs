using EmployeePerformance.Application.DTOs.Dashboard;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetActiveEmployeeCountAsync();

        Task<int> GetPendingReviewCountAsync();

        Task<int> GetCompletedReviewCountAsync();

        Task<decimal?> GetAverageApprovedRatingAsync();
    }
}
