using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.Employee
{
    public  class UpdateEmployeeDto
    {
        public string FristName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Department { get; set; } = null!;

        public string designation { get; set; } = null!;

        public int? ManagerId { get; set; }

        public DateOnly HireDate { get; set; }

        public bool IsActive { get; set; }
    }
}
