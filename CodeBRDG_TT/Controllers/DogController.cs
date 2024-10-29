using Microsoft.AspNetCore.Mvc;
using CodeBRDG_TT.Queries;
using MediatR;
using CodeBRDG_TT.Commands;

namespace CodeBRDG_TT.Controllers;

[ApiController]
public class DogController : ControllerBase
{
    private readonly IMediator _mediator;

    public DogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("dogs")]
    [HttpGet]
    public async Task<IActionResult> QueryDogs([FromQuery] DogsQuery query)
    {
        var dogs = await _mediator.Send(query);
        return Ok(dogs);
    }

    [Route("dog")]
    [HttpPost]
    public async Task<IActionResult> RegisterDog([FromBody] RegisterDogCommand command)
    {
        var result = await _mediator.Send(command);
        return result
            ? CreatedAtAction(nameof(QueryDogs), new { name = command.Name }, command)
            : BadRequest("Failed to create dog.");
    }
}
