using System.ComponentModel.DataAnnotations.Schema;

namespace College.DAL.Entities;

[Table("teachers")]
public class Teacher
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
   
    public string? Degree { get; set; }
    
    public List<Course> Courses { get; set; } = new();
}