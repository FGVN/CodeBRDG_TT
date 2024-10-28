using CodeBRDG_TT.Data.Repositories;
using CodeBRDG_TT.Models;

namespace CodeBRDG_TT.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<Dog> Dogs { get; }
    Task<int> SaveChangesAsync();
}
