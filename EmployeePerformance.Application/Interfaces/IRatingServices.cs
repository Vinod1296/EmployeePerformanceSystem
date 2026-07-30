using EmployeePerformance.Application.DTOs.Rating;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IRatingServices
    {
        Task<IEnumerable<RatingDto>> GetAllAsync();

        Task<RatingDto?> GetByIdAsync(int id);

        Task<RatingDto> AddAsync(CreateRatingDto dto);

        Task UpdateAsync(int id, UpdateRatingDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
