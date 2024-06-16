using College.DAL.Entities;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Courses;
using College.DAL.Repositories.Realizations.Base;

namespace College.DAL.Repositories.Realizations.Courses;

public class CoursesRepository : RepositoryBase<Course>, ICoursesRepository
{
    public CoursesRepository(CollegeDbContext context)
        : base(context)
    {
    }
}
