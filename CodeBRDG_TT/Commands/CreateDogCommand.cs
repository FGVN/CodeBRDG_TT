using MediatR;

namespace CodeBRDG_TT.Commands;

public class CreateDogCommand : IRequest<bool>
{
    public string Name { get; set; }
    public string Color { get; set; }
    public decimal TailLength { get; set; }
    public decimal Weight { get; set; }
}