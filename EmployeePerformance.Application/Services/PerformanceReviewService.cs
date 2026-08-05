using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.PerformanceReview;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class PerformanceReviewService : IPerformanceReviewService
    {
        private readonly IPerformanceReviewRepository _performanceReviewRepository;
        private readonly IRatingRepository _ratingRepository;

        public PerformanceReviewService(IPerformanceReviewRepository performanceReviewRepository, IRatingRepository ratingRepository)
        {
            _performanceReviewRepository = performanceReviewRepository;
            _ratingRepository = ratingRepository;
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
            if (await _performanceReviewRepository.ExistsByEmployeeAndCycleAsync(dto.EmployeeId, dto.ReviewCycleId))
            {
                throw new InvalidOperationException("A performance review already exists for this employee in this review cycle.");
            }

            var performanceReview = MapToEntity(dto);

            await _performanceReviewRepository.AddAsync(performanceReview);

            return MapToDto(performanceReview);
        }

        public async Task SubmitSelfAssessmentAsync(int reviewId, SubmitSelfAssessmentDto dto, CurrentUserContextDto currentUser)
        {
            if (!IsEmployee(currentUser))
            {
                throw new UnauthorizedAccessException("You are not authorized to update this review.");
            }

            var employeeId = currentUser.EmployeeId;
            var performanceReview = await _performanceReviewRepository.GetPerformanceReviewByIdAsync(reviewId);

            if (performanceReview == null)
            {
                throw new KeyNotFoundException("Performance Review not found.");
            }

            if (performanceReview.EmployeeId != employeeId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this review.");
            }

            if (IsReviewCycleClosed(performanceReview.ReviewCycle))
            {
                throw new ArgumentException("The review cycle is no longer accepting self assessments.");
            }

            if (!IsSelfAssessmentAllowed(performanceReview.Status))
            {
                throw new ArgumentException("Cannot submit self-assessment in the current status.");
            }

            ValidateSelfAssessment(dto);

            performanceReview.SelfAssessment = dto.SelfAssessment!.Trim();
            performanceReview.SubmittedDate = DateTime.UtcNow;
            performanceReview.Status = "Submitted";
            performanceReview.ModifiedAt = DateTime.UtcNow;

            await _performanceReviewRepository.SubmitSelfAssessmentAsync(performanceReview);
        }

        public async Task ManagerReviewAsync(int reviewId, ManagerReviewDto dto, CurrentUserContextDto currentUser)
        {
            if (!IsManager(currentUser))
            {
                throw new UnauthorizedAccessException("You are not authorized to review this performance review.");
            }

            if (dto == null)
            {
                throw new ArgumentException("Manager review request is required.");
            }

            if (!string.Equals(dto.Action, "Approve", StringComparison.Ordinal) &&
                !string.Equals(dto.Action, "NeedsRevision", StringComparison.Ordinal))
            {
                throw new ArgumentException("Invalid action value. Allowed: Approve, NeedsRevision.");
            }

            var performanceReview = await _performanceReviewRepository.GetPerformanceReviewByIdAsync(reviewId);

            if (performanceReview == null)
            {
                throw new KeyNotFoundException("Performance Review not found.");
            }

            if (performanceReview.ManagerId != currentUser.EmployeeId)
            {
                throw new UnauthorizedAccessException("You are not authorized to review this performance review.");
            }

            if (!IsManagerReviewAllowed(performanceReview.Status))
            {
                throw new ArgumentException("Performance review cannot be reviewed in the current status.");
            }

            ValidateManagerComments(dto.ManagerComments);

            performanceReview.ManagerComments = dto.ManagerComments!.Trim();

            if (string.Equals(dto.Action, "Approve", StringComparison.Ordinal))
            {
                var ratings = (await _ratingRepository.GetByPerformanceReviewIdAsync(reviewId)).ToList();
                if (ratings.Count == 0)
                {
                    throw new ArgumentException("No ratings found for this performance review.");
                }

                performanceReview.OverallRating = (decimal)Math.Round(ratings.Average(rating => rating.Score), 2, MidpointRounding.AwayFromZero);
                performanceReview.Status = "Approved";
                performanceReview.ApprovedDate = DateTime.UtcNow;
                performanceReview.ModifiedAt = DateTime.UtcNow;
            }
            else
            {
                performanceReview.Status = "NeedsRevision";
                performanceReview.ModifiedAt = DateTime.UtcNow;
            }

            await _performanceReviewRepository.UpdateManagerReviewAsync(performanceReview);
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
                Status = "Draft",
                SelfAssessment = null,
                ManagerComments = null,
                OverallRating = null,
                SubmittedDate = null,
                ApprovedDate = null,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = null
            };
        }

        private static void ValidateSelfAssessment(SubmitSelfAssessmentDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Self assessment is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.SelfAssessment))
            {
                throw new ArgumentException("Self assessment is required.");
            }

            if (dto.SelfAssessment.Length > 2000)
            {
                throw new ArgumentException("Self assessment must not exceed 2000 characters.");
            }
        }

        private static void ValidateManagerComments(string? managerComments)
        {
            if (string.IsNullOrWhiteSpace(managerComments))
            {
                throw new ArgumentException("Manager comments are required.");
            }

            if (managerComments.Length > 2000)
            {
                throw new ArgumentException("Manager comments must not exceed 2000 characters.");
            }
        }

        private static bool IsSelfAssessmentAllowed(string? status)
        {
            return string.Equals(status, "Draft", StringComparison.Ordinal)
                || string.Equals(status, "NeedsRevision", StringComparison.Ordinal);
        }

        private static bool IsManagerReviewAllowed(string? status)
        {
            return string.Equals(status, "Submitted", StringComparison.Ordinal)
                || string.Equals(status, "NeedsRevision", StringComparison.Ordinal);
        }

        private static bool IsReviewCycleClosed(ReviewCycle reviewCycle)
        {
            var status = reviewCycle?.Status?.Trim();
            if (string.IsNullOrWhiteSpace(status))
            {
                return true;
            }

            return status.Equals("Closed", StringComparison.OrdinalIgnoreCase)
                || status.Equals("Completed", StringComparison.OrdinalIgnoreCase)
                || status.Equals("Inactive", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEmployee(CurrentUserContextDto currentUser)
        {
            return string.Equals(currentUser.Role, "Employee", StringComparison.Ordinal);
        }

        private static bool IsManager(CurrentUserContextDto currentUser)
        {
            return string.Equals(currentUser.Role, "Manager", StringComparison.Ordinal);
        }
    }
}
