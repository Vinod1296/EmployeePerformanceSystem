using System;
using System.Collections.Generic;

namespace EmployeePerformance.Domain.Entities;

public partial class PerformanceReview
{
    public int PerformanceReviewId { get; set; }

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

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee Manager { get; set; } = null!;

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ReviewCycle ReviewCycle { get; set; } = null!;
}
