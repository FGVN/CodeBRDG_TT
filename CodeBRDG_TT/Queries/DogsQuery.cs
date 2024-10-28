using CodeBRDG_TT.Models;
using MediatR;

namespace CodeBRDG_TT.Queries;

public class DogsQuery : IRequest<List<Dog>>
{

    public string attribute { get; set; } = "name";
    public string order { get; set; } = "asc"; // Default order
    public int pageNumber { get; set; } = 1; // Default page number
    public int pageSize { get; set; } = 10; // Default page size
}
