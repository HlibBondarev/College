using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace College.DAL.Entities;

[Table("courses")]
public class Course
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int Duration { get; set; }

    public Guid TeacherId { get; set; }

    public Teacher? Teacher { get; set; }

    public List<Student> Students { get; set; } = new();
}