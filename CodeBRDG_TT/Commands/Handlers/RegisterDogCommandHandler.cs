using CodeBRDG_TT.Data.UnitOfWork;
using CodeBRDG_TT.Models;
using MediatR;

namespace CodeBRDG_TT.Commands.Handlers;

public class RegisterDogCommandHandler : IRequestHandler<RegisterDogCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterDogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RegisterDogCommand request, CancellationToken cancellationToken)
    {
        var dog = new Dog
        {
            name = request.name,
            color = request.color,
            tail_length = request.tail_length,
            weight = request.weight
        };

        await _unitOfWork.Dogs.AddAsync(dog); 

        await _unitOfWork.SaveChangesAsync();
        return true; 
    }
}
