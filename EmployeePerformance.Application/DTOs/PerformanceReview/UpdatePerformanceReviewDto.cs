using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.PerformanceReview
{
    public  class UpdatePerformanceReviewDto
    {
        public string ? SelfAssessment { get; set; }

        public string? ManagerComments { get; set; }

        public decimal? OverallRating { get; set; }

        public string Status { get; set; } = null!;
    }
}
