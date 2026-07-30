namespace EmployeePerformance.Application.DTOs.Auth
{
    /// <summary>
    /// Represents the response returned after creating a new user account.
    /// </summary>
    public class RegisterResponseDto
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
