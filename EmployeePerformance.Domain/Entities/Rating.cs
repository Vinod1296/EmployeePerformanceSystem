using System;
using System.Collections.Generic;

namespace EmployeePerformance.Domain.Entities;

public partial class Rating
{
    public int RatingId { get; set; }

    public int PerformanceReviewId { get; set; }

    public string Criteria { get; set; } = null!;

    public int Score { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual PerformanceReview PerformanceReview { get; set; } = null!;
}
