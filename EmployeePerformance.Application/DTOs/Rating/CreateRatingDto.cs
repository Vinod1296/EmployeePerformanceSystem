using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.Rating
{
    public class CreateRatingDto
    {
        public int performanceReviewId { get; set; }

        public string criteria { get; set; } = null!;

        public int score { get; set; }

        public string? comments { get; set; }
    }
}
