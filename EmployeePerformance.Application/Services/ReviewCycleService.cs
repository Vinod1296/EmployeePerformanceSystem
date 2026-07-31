using EmployeePerformance.Application.DTOs.ReviewCycle;
using EmployeePerformance.Application.Interfaces;
using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Services
{
    public class ReviewCycleService : IReviewCycleService
    {
        private readonly IReviewCycleRepository _reviewCycleRepository;

        public ReviewCycleService(IReviewCycleRepository reviewCycleRepository)
        {
            _reviewCycleRepository = reviewCycleRepository;
        }

        public async Task<IEnumerable<ReviewCycleDto>> GetAllReviewCyclesAsync()
        {
            var reviewCycles = await _reviewCycleRepository.GetAllAsync();
            return reviewCycles.Select(rc => new ReviewCycleDto
            {
                ReviewCycleId = rc.ReviewCycleId,
                CycleName = rc.CycleName,
                StartDate = rc.StartDate,
                EndDate = rc.EndDate,
                Status = rc.Status,
                CreatedByEmployeeId = rc.CreatedByEmployeeId
            });
        }

        public async Task<ReviewCycleDto?> GetReviewCycleByIdAsync(int id)
        {
            var reviewCycle = await _reviewCycleRepository.GetByIdAsync(id);
            if (reviewCycle == null) return null;
            return new ReviewCycleDto
            {
                ReviewCycleId = reviewCycle.ReviewCycleId,
                CycleName = reviewCycle.CycleName,
                StartDate = reviewCycle.StartDate,
                EndDate = reviewCycle.EndDate,
                Status = reviewCycle.Status,
                CreatedByEmployeeId = reviewCycle.CreatedByEmployeeId
            };
        }

        public async Task<ReviewCycleDto> AddReviewCycleAsync(CreateReviewCycleDto createReviewCycleDto)
        {
            var reviewCycle = new ReviewCycle
            {
                CycleName = createReviewCycleDto.CycleName,
                StartDate = createReviewCycleDto.StartDate,
                EndDate = createReviewCycleDto.EndDate,
                Status = createReviewCycleDto.Status,
                CreatedByEmployeeId = createReviewCycleDto.CreatedByEmployeeId,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewCycleRepository.AddAsync(reviewCycle);

            return new ReviewCycleDto
            {
                ReviewCycleId = reviewCycle.ReviewCycleId,
                CycleName = reviewCycle.CycleName,
                StartDate = reviewCycle.StartDate,
                EndDate = reviewCycle.EndDate,
                Status = reviewCycle.Status,
                CreatedByEmployeeId = reviewCycle.CreatedByEmployeeId
            };
        }

        public async Task UpdateReviewCycleAsync(int id, UpdateReviewCycleDto updateReviewCycleDto)
        {
            var reviewCycle = await _reviewCycleRepository.GetByIdAsync(id);
            if (reviewCycle == null) throw new KeyNotFoundException("Review cycle not found");
            reviewCycle.CycleName = updateReviewCycleDto.CycleName;
            reviewCycle.StartDate = updateReviewCycleDto.StartDate;
            reviewCycle.EndDate = updateReviewCycleDto.EndDate;
            reviewCycle.Status = updateReviewCycleDto.Status;

            await _reviewCycleRepository.UpdateAsync(reviewCycle);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _reviewCycleRepository.DeleteAsync(id);
        }
    }
}
