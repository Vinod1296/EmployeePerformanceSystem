using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.PerformanceReview
{
    public class CreatePerformanceReviewDto
    {
        public int ReviewCycleId { get; set; }
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
    }
}
