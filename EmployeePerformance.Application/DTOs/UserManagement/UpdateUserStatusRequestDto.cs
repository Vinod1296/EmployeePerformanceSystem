using System.ComponentModel.DataAnnotations;

namespace EmployeePerformance.Application.DTOs.UserManagement
{
    public class UpdateUserStatusRequestDto
    {
        [Required]
        public bool? IsActive { get; set; }
    }
}
