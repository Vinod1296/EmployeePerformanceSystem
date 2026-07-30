namespace EmployeePerformance.Application.DTOs.UserManagement
{
    public class UserQueryParametersDto
    {
        public string? Search { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "Username";

        public string SortDirection { get; set; } = "asc";
    }
}
