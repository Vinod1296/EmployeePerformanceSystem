namespace EmployeePerformance.Application.DTOs.UserManagement
{
    public class UserDetailsDto
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
