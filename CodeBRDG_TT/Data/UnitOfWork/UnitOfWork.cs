using CodeBRDG_TT.Data.Repositories;
using CodeBRDG_TT.Models;

namespace CodeBRDG_TT.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<Dog> _dogs;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        _dogs ??= new Repository<Dog>(_context);
    }

    public IRepository<Dog> Dogs => _dogs ??= new Repository<Dog>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
