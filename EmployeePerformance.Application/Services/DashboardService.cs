using EmployeePerformance.Application.DTOs.Dashboard;
using EmployeePerformance.Application.Interfaces;

namespace EmployeePerformance.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var totalEmployees = await _dashboardRepository.GetActiveEmployeeCountAsync();
            var pendingReviews = await _dashboardRepository.GetPendingReviewCountAsync();
            var completedReviews = await _dashboardRepository.GetCompletedReviewCountAsync();
            var averageRating = await _dashboardRepository.GetAverageApprovedRatingAsync();

            return new DashboardDto
            {
                totalEmployees = totalEmployees,
                pendingReviews = pendingReviews,
                completedReviews = completedReviews,
                averageRating = Math.Round(averageRating ?? 0m, 2, MidpointRounding.AwayFromZero)
            };
        }
    }
}
