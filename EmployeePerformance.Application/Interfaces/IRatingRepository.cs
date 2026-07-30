using EmployeePerformance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();

        Task<Rating?> GetByIdAsync(int id);

        Task AddAsync(Rating rating);

        Task UpdateAsync(Rating rating);

        Task<bool> DeleteAsync(int id);
    }
}
