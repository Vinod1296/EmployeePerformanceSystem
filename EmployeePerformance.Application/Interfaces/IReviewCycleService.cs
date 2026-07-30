using EmployeePerformance.Application.DTOs.ReviewCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public  interface IReviewCycleService
    {
        Task<IEnumerable<ReviewCycleDto>> GetAllReviewCyclesAsync();
        Task<ReviewCycleDto?> GetReviewCycleByIdAsync(int id);
        Task<ReviewCycleDto> AddReviewCycleAsync(CreateReviewCycleDto createReviewCycleDto);
        Task UpdateReviewCycleAsync(int id, UpdateReviewCycleDto updateReviewCycleDto);
        Task<bool> DeleteAsync(int id);

    }
}
