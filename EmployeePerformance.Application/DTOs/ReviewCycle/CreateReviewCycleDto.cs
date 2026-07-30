using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.ReviewCycle
{
    public  class CreateReviewCycleDto
    {
       public string CycleName { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = null!;

        public int CreatedByEmployeeId { get; set; }
    }
}
