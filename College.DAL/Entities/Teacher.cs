using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace College.DAL.Entities;

[Table("teachers")]
public class Teacher
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(60, ErrorMessage = "Name can't be longer than 60 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Degree is required")]
    [StringLength(40, ErrorMessage = "Degree can't be longer than 40 characters")]
    
    public string? Degree { get; set; }
    
    public List<Course> Courses { get; set; } = new();
}