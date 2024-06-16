using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace College.DAL.Repositories.Interfaces.Base;

public interface IRepositoryBase<T>
    where T : class
{
    public Task<IEnumerable<T>> GetAllAsync(
    Expression<Func<T, bool>>? predicate = default,
    Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default);

    public Task<IEnumerable<T>?> GetAllAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default);

    public Task<T?> GetSingleOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default);

    public Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default);

    public Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, T>> selector,
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default);

    public IQueryable<T> Include(params Expression<Func<T, object>>[] includes);

    public T Create(T entity);

    public Task<T> CreateAsync(T entity);

    public EntityEntry<T> Update(T entity);

    public void Delete(T entity);
}

