namespace College.BLL.DTO.Courses;

public class UpdateCourseResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int? Duration { get; set; }

    public Guid TeacherId { get; set; }

    public List<Guid>? CourseStudents { get; set; }
}