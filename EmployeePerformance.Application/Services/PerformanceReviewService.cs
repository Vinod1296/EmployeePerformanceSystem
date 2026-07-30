using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.PerformanceReview;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class PerformanceReviewService : IPerformanceReviewService
    {
        private readonly IPerformanceReviewRepository _performanceReviewRepository;

        public PerformanceReviewService(IPerformanceReviewRepository performanceReviewRepository)
        {
            _performanceReviewRepository = performanceReviewRepository;
        }

        public async Task<IEnumerable<PerformanceReviewDto>> GetAllAsync(CurrentUserContextDto currentUser)
        {
            var performanceReviews = IsEmployee(currentUser)
                ? await _performanceReviewRepository.GetByEmployeeIdAsync(currentUser.EmployeeId)
                : await _performanceReviewRepository.GetAllAsync();

            return performanceReviews.Select(MapToDto);
        }

        public async Task<PerformanceReviewDto?> GetByIdAsync(int id, CurrentUserContextDto currentUser)
        {
            var performanceReview = IsEmployee(currentUser)
                ? await _performanceReviewRepository.GetByIdForEmployeeAsync(id, currentUser.EmployeeId)
                : await _performanceReviewRepository.GetByIdAsync(id);

            if (performanceReview == null)
                return null;

            return MapToDto(performanceReview);
        }

        public async Task<PerformanceReviewDto> AddAsync(CreatePerformanceReviewDto dto)
        {
            var performanceReview = MapToEntity(dto);

            await _performanceReviewRepository.AddAsync(performanceReview);

            return MapToDto(performanceReview);
        }

        public async Task UpdateAsync(int id, UpdatePerformanceReviewDto dto)
        {
            var performanceReview = await _performanceReviewRepository.GetByIdAsync(id);

            if (performanceReview == null)
                throw new KeyNotFoundException("Performance Review not found.");

            UpdateEntity(performanceReview, dto);

            await _performanceReviewRepository.UpdateAsync(performanceReview);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _performanceReviewRepository.DeleteAsync(id);
        }

        private PerformanceReviewDto MapToDto(PerformanceReview performanceReview)
        {
            return new PerformanceReviewDto
            {
                performanceReviewId = performanceReview.PerformanceReviewId,
                ReviewCycleId = performanceReview.ReviewCycleId,
                EmployeeId = performanceReview.EmployeeId,
                ManagerId = performanceReview.ManagerId,
                SelfAssessment = performanceReview.SelfAssessment,
                ManagerComments = performanceReview.ManagerComments,
                OverallRating = performanceReview.OverallRating,
                Status = performanceReview.Status,
                SubmittedDate = performanceReview.SubmittedDate,
                ApprovedDate = performanceReview.ApprovedDate,
                CreatedAt = performanceReview.CreatedAt,
                ModifiedAt = performanceReview.ModifiedAt
            };
        }

        private PerformanceReview MapToEntity(CreatePerformanceReviewDto dto)
        {
            return new PerformanceReview
            {
                ReviewCycleId = dto.ReviewCycleId,
                EmployeeId = dto.EmployeeId,
                ManagerId = dto.ManagerId,
                SelfAssessment = dto.SelfAssessment,
                ManagerComments = dto.ManagerComments,
                OverallRating = dto.OverallRating,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow
            };
        }

        private void UpdateEntity(PerformanceReview performanceReview, UpdatePerformanceReviewDto dto)
        {
            performanceReview.SelfAssessment = dto.SelfAssessment;
            performanceReview.ManagerComments = dto.ManagerComments;
            performanceReview.OverallRating = dto.OverallRating;
            performanceReview.Status = dto.Status;
            performanceReview.ModifiedAt = DateTime.UtcNow;
        }

        private static bool IsEmployee(CurrentUserContextDto currentUser)
        {
            return string.Equals(currentUser.Role, "Employee", StringComparison.Ordinal);
        }
    }
}
