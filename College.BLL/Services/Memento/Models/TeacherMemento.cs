using College.DAL.Entities;

namespace College.BLL.Services.Memento.Models;

public class TeacherMemento
{
    public string? Name { get; set; }
    public string? Degree { get; set; }
    public List<Course> Courses { get; set; } = new();
}