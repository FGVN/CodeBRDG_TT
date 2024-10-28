using MediatR;

namespace CodeBRDG_TT.Commands;

public class RegisterDogCommand : IRequest<bool>
{
    public string name { get; set; }
    public string color { get; set; }
    public decimal tail_length { get; set; }
    public decimal weight { get; set; }
}