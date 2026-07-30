using System.ComponentModel.DataAnnotations;

namespace EmployeePerformance.Application.DTOs.Auth
{
    /// <summary>
    /// Represents the payload used to register a new user account.
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Gets or sets the employee identifier linked to the user account.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeId must be a valid positive number.")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the unique username for the account.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the plain text password to hash before persistence.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

    }
}
