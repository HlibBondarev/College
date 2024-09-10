using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace College.BLL.DTO.Teachers.Drafts;

[JsonDerivedType(typeof(TeacherWithNameDto), typeDiscriminator: "withName")]
[JsonDerivedType(typeof(TeacherWithDegreeDto), typeDiscriminator: "withDegree")]
public class TeacherWithNameDto
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
}