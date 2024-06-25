namespace College.BLL.DTO.Students;

public class UpdateStudentResponseDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    
    public List<Guid>? StudentCourses { get; set; }
}