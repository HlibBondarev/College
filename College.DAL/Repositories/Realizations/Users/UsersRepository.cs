using College.DAL.Entities.JwtAuthentication;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Users;
using College.DAL.Repositories.Realizations.Base;

namespace College.DAL.Repositories.Realizations.Users;

public class UsersRepository : RepositoryBase<ApplicationUser>, IUsersRepository
{
    public UsersRepository(CollegeDbContext context)
        : base(context)
    {
    }
}