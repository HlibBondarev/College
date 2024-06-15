using System.ComponentModel.DataAnnotations;

namespace College.DAL.Entities;

public class StudentCourse
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid CourseId { get; set; }

    public Student? Student { get; set; }

    public Course? Course { get; set; }
}
