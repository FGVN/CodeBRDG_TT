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
        if (request.Weight < 0)
            throw new ArgumentOutOfRangeException(nameof(request.Weight), "Weight must be greater than or equal to zero.");
        

        if (request.TailLength < 0)
            throw new ArgumentOutOfRangeException(nameof(request.TailLength), "Tail length must be greater than or equal to zero.");
        

        var dog = new Dog( request.Name, request.Color, request.TailLength, request.Weight);

        await _unitOfWork.Dogs.AddAsync(dog); 

        await _unitOfWork.SaveChangesAsync();
        return true; 
    }
}
