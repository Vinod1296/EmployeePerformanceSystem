using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.Employee
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; } = null!;

        public string FristName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public String Department { get; set; } = null!;

        public string Designation { get; set; } = null!;

        public int? ManagerId { get; set; }

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; }
    }
}
