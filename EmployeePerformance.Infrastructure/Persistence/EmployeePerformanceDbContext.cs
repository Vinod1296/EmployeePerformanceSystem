using EmployeePerformance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EmployeePerformance.Infrastructure.Persistence;

public partial class EmployeePerformanceDbContext : DbContext
{
    public EmployeePerformanceDbContext()
    {
    }

    public EmployeePerformanceDbContext(DbContextOptions<EmployeePerformanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<ReviewCycle> ReviewCycles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11B156757B");

            entity.HasIndex(e => e.Department, "IX_Employees_Department");

            entity.HasIndex(e => e.IsActive, "IX_Employees_IsActive");

            entity.HasIndex(e => e.ManagerId, "IX_Employees_ManagerId");

            entity.HasIndex(e => e.EmployeeCode, "UQ__Employee__1F642548D5F2BACE").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Employee__A9D10534E7C20904").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.EmployeeCode).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_Employees_Manager");
        });

        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.HasKey(e => e.PerformanceReviewId).HasName("PK__Performa__F8895D16DBBEF9CC");

            entity.HasIndex(e => e.EmployeeId, "IX_PerfReviews_EmployeeId");

            entity.HasIndex(e => e.ManagerId, "IX_PerfReviews_ManagerId");

            entity.HasIndex(e => e.ReviewCycleId, "IX_PerfReviews_ReviewCycleId");

            entity.HasIndex(e => e.Status, "IX_PerfReviews_Status");

            entity.HasIndex(e => new { e.ReviewCycleId, e.EmployeeId }, "UQ_PerfReviews_Employee_Cycle").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.OverallRating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceReviewEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PerfReviews_Employee");

            entity.HasOne(d => d.Manager).WithMany(p => p.PerformanceReviewManagers)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PerfReviews_Manager");

            entity.HasOne(d => d.ReviewCycle).WithMany(p => p.PerformanceReviews)
                .HasForeignKey(d => d.ReviewCycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PerfReviews_ReviewCycle");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF87C84E13747");

            entity.HasIndex(e => e.PerformanceReviewId, "IX_Ratings_PerformanceReviewId");

            entity.Property(e => e.Comments).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Criteria).HasMaxLength(150);

            entity.HasOne(d => d.PerformanceReview).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.PerformanceReviewId)
                .HasConstraintName("FK_Ratings_PerformanceReview");
        });

        modelBuilder.Entity<ReviewCycle>(entity =>
        {
            entity.HasKey(e => e.ReviewCycleId).HasName("PK__ReviewCy__03F9D692D8AC857A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CycleName).HasMaxLength(150);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");

            entity.HasOne(d => d.CreatedByEmployee).WithMany(p => p.ReviewCycles)
                .HasForeignKey(d => d.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewCycles_CreatedBy");
        });


        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.Username)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.Role)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Employee)
                  .WithMany(p => p.Users)
                  .HasForeignKey(d => d.EmployeeId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Users_Employees");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
