namespace EmployeePerformance.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
