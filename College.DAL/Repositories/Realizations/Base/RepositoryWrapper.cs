using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Base;
using College.DAL.Repositories.Interfaces.Courses;
using College.DAL.Repositories.Interfaces.Students;
using College.DAL.Repositories.Interfaces.Teachers;
using College.DAL.Repositories.Interfaces.Users;
using College.DAL.Repositories.Realizations.Courses;
using College.DAL.Repositories.Realizations.Students;
using College.DAL.Repositories.Realizations.Teachers;
using College.DAL.Repositories.Realizations.Users;
using System.Transactions;

namespace College.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly CollegeDbContext _collegeDbContext;

    private ICoursesRepository _coursesRepository;

    private IStudentCourseRepository _studentCourseRepository;

    private IStudentsRepository _studentsRepository;

    private ITeachersRepository _teachersRepository;

    private IUsersRepository _usersRepository;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public RepositoryWrapper(CollegeDbContext collegeDbContext)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _collegeDbContext = collegeDbContext;
    }

    public ICoursesRepository CoursesRepository
    {
        get
        {
            _coursesRepository ??= new CoursesRepository(_collegeDbContext);

            return _coursesRepository;
        }
    }

    public IStudentCourseRepository StudentCourseRepository
    {
        get
        {
            _studentCourseRepository ??= new StudentCourseRepository(_collegeDbContext);

            return _studentCourseRepository;
        }
    }

    public IStudentsRepository StudentsRepository
    {
        get
        {
            _studentsRepository ??= new StudentsRepository(_collegeDbContext);

            return _studentsRepository;
        }
    }

    public ITeachersRepository TeachersRepository
    {
        get
        {
            _teachersRepository ??= new TeachersRepository(_collegeDbContext);

            return _teachersRepository;
        }
    }

    public IUsersRepository UsersRepository
    {
        get
        {
            _usersRepository ??= new UsersRepository(_collegeDbContext);

            return _usersRepository;
        }
    }

    public int SaveChanges()
    {
        return _collegeDbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _collegeDbContext.SaveChangesAsync();
    }

    public void Update<TEntity>(TEntity entity) where TEntity : class
    {
        _collegeDbContext.Update(entity);
    }

    public TransactionScope BeginTransaction()
    {
        return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }
}
