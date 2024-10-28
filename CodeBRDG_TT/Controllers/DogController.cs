using Microsoft.AspNetCore.Mvc;
using CodeBRDG_TT.Queries;
using MediatR;
using CodeBRDG_TT.Commands;

namespace CodeBRDG_TT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DogController : ControllerBase
{
    private readonly IMediator _mediator;

    public DogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("dogs")]
    [HttpGet]
    public async Task<IActionResult> GetDogs()
    {
        var query = new DogsQuery();
        var dogs = await _mediator.Send(query);
        return Ok(dogs);
    }

    [Route("dog")]
    [HttpPost]
    public async Task<IActionResult> CreateDog([FromBody] CreateDogCommand command)
    {
        if (command == null)
        {
            return BadRequest("Dog data is required.");
        }

        var result = await _mediator.Send(command);
        if (result)
        {
            return CreatedAtAction(nameof(GetDogs), new { name = command.Name }, command);
        }

        return BadRequest("Failed to create dog.");
    }
}
