namespace EmployeePerformance.Application.DTOs.Common
{
    public class CurrentUserContextDto
    {
        public int EmployeeId { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
