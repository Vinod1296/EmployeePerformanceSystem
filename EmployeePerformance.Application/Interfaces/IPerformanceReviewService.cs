using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.PerformanceReview;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IPerformanceReviewService
    {
        Task<IEnumerable<PerformanceReviewDto>> GetAllAsync(CurrentUserContextDto currentUser);

        Task<PerformanceReviewDto?> GetByIdAsync(int id, CurrentUserContextDto currentUser);

        Task<PerformanceReviewDto> AddAsync(CreatePerformanceReviewDto dto);

        Task UpdateAsync(int id, UpdatePerformanceReviewDto dto);

        Task SubmitSelfAssessmentAsync(int reviewId, SubmitSelfAssessmentDto dto, CurrentUserContextDto currentUser);

        Task ManagerReviewAsync(int reviewId, ManagerReviewDto dto, CurrentUserContextDto currentUser);

        Task<bool> DeleteAsync(int id);
    }
}
