using System;
using System.Collections.Generic;

namespace EmployeePerformance.Domain.Entities;

public partial class ReviewCycle
{
    public int ReviewCycleId { get; set; }

    public string CycleName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = null!;

    public int CreatedByEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Employee CreatedByEmployee { get; set; } = null!;

    public virtual ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
}
