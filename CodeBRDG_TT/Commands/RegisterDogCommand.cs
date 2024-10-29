using MediatR;

namespace CodeBRDG_TT.Commands;

public class RegisterDogCommand : IRequest<bool>
{
    public string Name { get; set; }
    public string Color { get; set; }
    public decimal TailLength { get; set; }
    public decimal Weight { get; set; }

    public RegisterDogCommand(string name, string color, decimal tailLength, decimal weight)
    {
        Name = name;
        Color = color;
        TailLength = tailLength;
        Weight = weight;
    }
}
