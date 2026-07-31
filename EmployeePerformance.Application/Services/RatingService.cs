using EmployeePerformance.Application.DTOs.Rating;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class RatingService : IRatingServices
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
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

        public async Task<RatingDto> AddAsync(CreateRatingDto dto)
        {
            var rating = MapToEntity(dto);

            await _ratingRepository.AddAsync(rating);

            return MapToDto(rating);
        }

        public async Task UpdateAsync(int id, UpdateRatingDto dto)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);

            if (rating == null)
                throw new KeyNotFoundException("Rating not found.");

            UpdateEntity(rating, dto);

            await _ratingRepository.UpdateAsync(rating);
        }

        public async Task<bool> DeleteAsync(int id)
        {
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
    }
}
