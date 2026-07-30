using System.ComponentModel.DataAnnotations;

namespace EmployeePerformance.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
