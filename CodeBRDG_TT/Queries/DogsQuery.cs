using CodeBRDG_TT.Models;
using MediatR;

namespace CodeBRDG_TT.Queries;

public class DogsQuery : IRequest<List<Dog>>
{
    public string Attribute { get; set; } = "name";
    public string Order { get; set; } = "asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}