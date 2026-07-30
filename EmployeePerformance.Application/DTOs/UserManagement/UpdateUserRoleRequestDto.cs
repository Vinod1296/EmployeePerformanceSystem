using System.ComponentModel.DataAnnotations;

namespace EmployeePerformance.Application.DTOs.UserManagement
{
    public class UpdateUserRoleRequestDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
