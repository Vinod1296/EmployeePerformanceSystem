using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.Employee
{
    public class EmployeeSearchDto
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Department { get; set; }

        //Pagination properties
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        //sorting properties

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; } = "asc";
    }
}
