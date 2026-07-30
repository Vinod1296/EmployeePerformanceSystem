using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.PerformanceReview
{
    public class PerformanceReviewDto
    {
        public int performanceReviewId { get; set;  } = 0;

        public int ReviewCycleId { get; set; }

        public int EmployeeId { get; set; }

        public int ManagerId { get; set; }

        public string? SelfAssessment { get; set; }

        public string? ManagerComments { get; set; }

        public decimal? OverallRating { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? SubmittedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
