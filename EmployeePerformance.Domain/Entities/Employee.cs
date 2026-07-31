using System;
using System.Collections.Generic;

namespace EmployeePerformance.Domain.Entities;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Department { get; set; }

    public string? Designation { get; set; }

    public int? ManagerId { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual Employee? Manager { get; set; }

    public virtual ICollection<PerformanceReview> PerformanceReviewEmployees { get; set; } = new List<PerformanceReview>();

    public virtual ICollection<PerformanceReview> PerformanceReviewManagers { get; set; } = new List<PerformanceReview>();

    public virtual ICollection<ReviewCycle> ReviewCycles { get; set; } = new List<ReviewCycle>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
