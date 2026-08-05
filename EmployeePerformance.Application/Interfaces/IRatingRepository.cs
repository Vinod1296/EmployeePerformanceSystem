using EmployeePerformance.Domain.Entities;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();

        Task<Rating?> GetByIdAsync(int id);

        Task<IEnumerable<Rating>> GetByPerformanceReviewIdAsync(int performanceReviewId);

        Task<bool> ExistsByPerformanceReviewIdAsync(int performanceReviewId);

        Task AddAsync(Rating rating);

        Task UpdateAsync(Rating rating);

        Task<bool> DeleteAsync(int id);
    }
}
