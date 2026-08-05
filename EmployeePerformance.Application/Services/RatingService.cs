using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.Rating;
using EmployeePerformance.Application.Exceptions;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class RatingService : IRatingServices
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IPerformanceReviewRepository _performanceReviewRepository;

        public RatingService(IRatingRepository ratingRepository, IPerformanceReviewRepository performanceReviewRepository)
        {
            _ratingRepository = ratingRepository;
            _performanceReviewRepository = performanceReviewRepository;
        }

        public async Task<IEnumerable<RatingDto>> GetAllAsync()
        {
            var ratings = await _ratingRepository.GetAllAsync();
            return ratings.Select(MapToDto);
        }

        public async Task<RatingDto?> GetByIdAsync(int id)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);

            if (rating == null)
                return null;

            return MapToDto(rating);
        }

        public async Task<RatingDto> AddAsync(CreateRatingDto dto, CurrentUserContextDto currentUser)
        {
            await ValidateRatingWorkflowAsync(dto.performanceReviewId, currentUser);

            var rating = MapToEntity(dto);

            await _ratingRepository.AddAsync(rating);

            return MapToDto(rating);
        }

        public async Task UpdateAsync(int id, UpdateRatingDto dto, CurrentUserContextDto currentUser)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);

            if (rating == null)
                throw new KeyNotFoundException("Rating not found.");

            await ValidateRatingWorkflowAsync(rating.PerformanceReviewId, currentUser);

            UpdateEntity(rating, dto);

            await _ratingRepository.UpdateAsync(rating);
        }

        public async Task<bool> DeleteAsync(int id, CurrentUserContextDto currentUser)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);

            if (rating == null)
                return false;

            await ValidateRatingWorkflowAsync(rating.PerformanceReviewId, currentUser);

            return await _ratingRepository.DeleteAsync(id);
        }

        private RatingDto MapToDto(Rating rating)
        {
            return new RatingDto
            {
                RatingId = rating.RatingId,
                PerformanceReviewId = rating.PerformanceReviewId,
                Criteria = rating.Criteria,
                Score = rating.Score,
                Comments = rating.Comments,
                CreatedAt = rating.CreatedAt
            };
        }

        private Rating MapToEntity(CreateRatingDto dto)
        {
            return new Rating
            {
                PerformanceReviewId = dto.performanceReviewId,
                Criteria = dto.criteria,
                Score = dto.score,
                Comments = dto.comments,
                CreatedAt = DateTime.UtcNow
            };
        }

        private void UpdateEntity(Rating rating, UpdateRatingDto dto)
        {
            rating.PerformanceReviewId = dto.performanceReviewId;
            rating.Criteria = dto.criteria;
            rating.Score = dto.score;
            rating.Comments = dto.comments;
        }

        private async Task ValidateRatingWorkflowAsync(int performanceReviewId, CurrentUserContextDto currentUser)
        {
            if (!IsManager(currentUser))
            {
                throw new ForbiddenException("Only the assigned manager may modify ratings.");
            }

            var performanceReview = await _performanceReviewRepository.GetPerformanceReviewByIdAsync(performanceReviewId);

            if (performanceReview == null)
            {
                throw new KeyNotFoundException("Performance Review not found.");
            }

            if (performanceReview.ManagerId != currentUser.EmployeeId)
            {
                throw new ForbiddenException("Only the assigned manager may modify ratings.");
            }

            if (!IsRatingModificationAllowed(performanceReview.Status))
            {
                throw new ArgumentException("Ratings can only be modified when the review status is Submitted or NeedsRevision.");
            }
        }

        private static bool IsManager(CurrentUserContextDto currentUser)
        {
            return string.Equals(currentUser.Role, "Manager", StringComparison.Ordinal);
        }

        private static bool IsRatingModificationAllowed(string? status)
        {
            return string.Equals(status, "Submitted", StringComparison.Ordinal)
                || string.Equals(status, "NeedsRevision", StringComparison.Ordinal);
        }
    }
}
