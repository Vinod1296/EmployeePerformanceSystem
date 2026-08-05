using EmployeePerformance.Application.DTOs.Common;
using EmployeePerformance.Application.DTOs.Rating;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IRatingServices
    {
        Task<IEnumerable<RatingDto>> GetAllAsync();

        Task<RatingDto?> GetByIdAsync(int id);

        Task<RatingDto> AddAsync(CreateRatingDto dto, CurrentUserContextDto currentUser);

        Task UpdateAsync(int id, UpdateRatingDto dto, CurrentUserContextDto currentUser);

        Task<bool> DeleteAsync(int id, CurrentUserContextDto currentUser);
    }
}
