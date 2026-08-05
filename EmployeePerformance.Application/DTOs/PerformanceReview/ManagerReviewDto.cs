namespace EmployeePerformance.Application.DTOs.PerformanceReview
{
    public sealed class ManagerReviewDto
    {
        public string Action { get; set; } = null!;

        public string? ManagerComments { get; set; }
    }
}
