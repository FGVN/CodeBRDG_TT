using CodeBRDG_TT.Data;
using CodeBRDG_TT.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodeBRDG_TT.Queries.Handlers;

public class DogsQueryHandler : IRequestHandler<DogsQuery, List<Dog>>
{
    private readonly AppDbContext _context;

    public DogsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dog>> Handle(DogsQuery query, CancellationToken cancellationToken)
    {
        return await _context.Dogs.ToListAsync(cancellationToken);
    }
}
