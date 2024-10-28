using CodeBRDG_TT.Models;
using MediatR;

namespace CodeBRDG_TT.Queries
{
    public class DogsQuery : IRequest<List<Dog>>
    {
    }
}
