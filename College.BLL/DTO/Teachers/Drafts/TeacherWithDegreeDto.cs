using System.ComponentModel.DataAnnotations;

namespace College.BLL.DTO.Teachers.Drafts;

public class TeacherWithDegreeDto: TeacherWithNameDto
{
    [Required(ErrorMessage = "Degree is required")]
    public string? Degree { get; set; }
}