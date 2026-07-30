using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeePerformance.Application.DTOs.Rating
{
    public class RatingDto
    {
        public int RatingId { get; set; }

        public int PerformanceReviewId { get; set; } = 0;


        [Required]
       [StringLength(150)]
       public string Criteria { get; set; } = null!;

        [Range(1, 5)]
        public int Score { get; set; } = 0;

        [StringLength(1000)]
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
