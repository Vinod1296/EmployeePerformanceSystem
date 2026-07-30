namespace EmployeePerformance.Application.DTOs.UserManagement
{
    public class UpdateUserRoleResponseDto
    {
        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string OldRole { get; set; } = string.Empty;

        public string NewRole { get; set; } = string.Empty;
    }
}
