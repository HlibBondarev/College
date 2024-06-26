using College.BLL.DTO.Students;

namespace College.BLL.DTO.Courses;

public class GetByIdCourseResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int Duration { get; set; }

    public string? TeacherName { get; set; }

    public IEnumerable<StudentDto>? Students { get; set; }
}