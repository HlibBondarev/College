using System.Transactions;
using College.DAL.Repositories.Interfaces.Courses;
using College.DAL.Repositories.Interfaces.Students;
using College.DAL.Repositories.Interfaces.Teachers;
using College.DAL.Repositories.Interfaces.Users;

namespace College.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public ICoursesRepository CoursesRepository { get; }
    public IStudentCourseRepository StudentCourseRepository { get; }
    public IStudentsRepository StudentsRepository { get; }
    public ITeachersRepository TeachersRepository { get; }
    public IUsersRepository UsersRepository {  get; }

    public int SaveChanges();

    public Task<int> SaveChangesAsync();

    public void Update<TEntity>(TEntity entity) where TEntity : class;

    public TransactionScope BeginTransaction();
}
