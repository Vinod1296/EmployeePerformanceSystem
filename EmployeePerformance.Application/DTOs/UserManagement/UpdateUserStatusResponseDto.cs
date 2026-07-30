namespace EmployeePerformance.Application.DTOs.UserManagement
{
    public class UpdateUserStatusResponseDto
    {
        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public bool OldStatus { get; set; }

        public bool NewStatus { get; set; }
    }
}
