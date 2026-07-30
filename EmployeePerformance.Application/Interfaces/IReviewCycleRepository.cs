using EmployeePerformance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public  interface IReviewCycleRepository
    {

        Task <IEnumerable<ReviewCycle>> GetAllAsync();

        Task<ReviewCycle?> GetByIdAsync(int id);

        Task AddAsync(ReviewCycle reviewCycle);

        Task<bool> DeleteAsync(int id);

        Task UpdateAsync(ReviewCycle reviewCycle);

    }
}
