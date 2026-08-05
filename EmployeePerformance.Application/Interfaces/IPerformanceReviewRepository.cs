using EmployeePerformance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public  interface IPerformanceReviewRepository
    {
        Task<IEnumerable<PerformanceReview>> GetAllAsync();

        Task<IEnumerable<PerformanceReview>> GetByEmployeeIdAsync(int employeeId);

        Task <PerformanceReview?> GetByIdAsync (int id);

        Task<PerformanceReview?> GetPerformanceReviewByIdAsync(int id);

        Task<PerformanceReview?> GetByIdForEmployeeAsync(int id, int employeeId);

        Task<bool> ExistsByEmployeeAndCycleAsync(int employeeId, int reviewCycleId);

        Task AddAsync(PerformanceReview performanceReview);

        Task SubmitSelfAssessmentAsync(PerformanceReview performanceReview);

        Task UpdateManagerReviewAsync(PerformanceReview performanceReview);
    
        Task <bool> DeleteAsync(int id);
    }
}
