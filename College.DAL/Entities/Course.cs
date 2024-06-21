using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace College.DAL.Entities;

[Table("courses")]
public class Course
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name of course is required")]
    [StringLength(40, ErrorMessage = "Course name can't be longer than 40 characters")]
    public string? Name { get; set; }

    [Required]
    public int Duration { get; set; }

    [Required]
    public Guid TeacherId { get; set; }

    public Teacher? Teacher { get; set; }

    public List<Student> Students { get; set; } = new();
}