using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CodeBRDG_TT.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByNameAsync(string name)
    {
        return await _dbSet.FindAsync(name);
    }

    // No eager loading for getall and find
    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate); 
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}
