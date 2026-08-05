namespace EmployeePerformance.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int totalEmployees { get; set; }
        public int pendingReviews { get; set; }
        public int completedReviews { get; set; }
        public decimal averageRating { get; set; }
    }
}
