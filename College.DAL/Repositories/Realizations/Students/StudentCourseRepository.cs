using College.DAL.Entities;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Students;
using College.DAL.Repositories.Realizations.Base;

namespace College.DAL.Repositories.Realizations.Students;

public class StudentCourseRepository : RepositoryBase<StudentCourse>, IStudentCourseRepository
{
    public StudentCourseRepository(CollegeDbContext context)
        : base(context)
    {
    }
}
