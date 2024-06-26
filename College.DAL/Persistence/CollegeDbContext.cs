﻿using College.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace College.DAL.Persistence;

public class CollegeDbContext : DbContext
{
    public CollegeDbContext()
    {
    }

    public CollegeDbContext(DbContextOptions<CollegeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course>? Courses { get; set; }
    public DbSet<Student>? Students { get; set; }
    public DbSet<StudentCourse>? StudentCourses { get; set; }
    public DbSet<Teacher>? Teachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasMany(d => d.Students)
                    .WithMany(p => p.Courses)
                    .UsingEntity<StudentCourse>(
                        sp => sp.HasOne(i => i.Student).WithMany().HasForeignKey(x => x.StudentId),
                        sp => sp.HasOne(i => i.Course).WithMany().HasForeignKey(x => x.CourseId))
                   .ToTable("student_course");
        });
    }
}
