using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College.DAL.Entities;

[Table("students")]
public class Student
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }

    public DateTime DateOfBirth { get; set; }

    public List<Course> Courses { get; set; } = new();
}