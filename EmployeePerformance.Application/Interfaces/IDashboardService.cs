using EmployeePerformance.Application.DTOs.Dashboard;

namespace EmployeePerformance.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}
