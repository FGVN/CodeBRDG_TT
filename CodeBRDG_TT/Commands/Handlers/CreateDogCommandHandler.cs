using CodeBRDG_TT.Data;
using CodeBRDG_TT.Models;
using MediatR;

namespace CodeBRDG_TT.Commands.Handlers;

public class CreateDogCommandHandler : IRequestHandler<CreateDogCommand, bool>
{
    private readonly AppDbContext _context;

    public CreateDogCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateDogCommand request, CancellationToken cancellationToken)
    {
        var dog = new Dog
        {
            name = request.Name,
            color = request.Color,
            tail_length = request.TailLength,
            weight = request.Weight
        };

        _context.Dogs.Add(dog);
        await _context.SaveChangesAsync(cancellationToken);
        return true; // Return true to indicate success
    }
}
