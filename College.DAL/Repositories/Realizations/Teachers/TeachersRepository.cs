using College.DAL.Entities;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Teachers;
using College.DAL.Repositories.Realizations.Base;

namespace College.DAL.Repositories.Realizations.Teachers;

public class TeachersRepository : RepositoryBase<Teacher>, ITeachersRepository
{
    public TeachersRepository(CollegeDbContext context)
        : base(context)
    {
    }
}