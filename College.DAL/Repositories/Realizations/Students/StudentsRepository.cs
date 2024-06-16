using College.DAL.Entities;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Students;
using College.DAL.Repositories.Realizations.Base;

namespace College.DAL.Repositories.Realizations.Students;

public class StudentsRepository : RepositoryBase<Student>, IStudentsRepository
{
    public StudentsRepository(CollegeDbContext context)
        : base(context)
    {
    }
}
