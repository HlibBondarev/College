using College.DAL.Repositories.Interfaces.Courses;
using College.DAL.Repositories.Interfaces.Students;
using College.DAL.Repositories.Interfaces.Teachers;
using System.Transactions;

namespace College.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public ICoursesRepository CoursesRepository { get; }
    public IStudentCourseRepository StudentCourseRepository { get; }
    public IStudentsRepository StudentsRepository { get; }
    public ITeachersRepository TeachersRepository { get; }

    public int SaveChanges();

    public Task<int> SaveChangesAsync();

    public TransactionScope BeginTransaction();
}
