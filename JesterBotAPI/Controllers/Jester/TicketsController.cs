using Asp.Versioning;
using DellArteAPI.Models.V1.Jester;
using DellArteAPI.Services.Jester;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Jester;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]")]
public class TicketsController(ITicketsService ticketsService) : ControllerBase
{
    private readonly ITicketsService _ticketsService = ticketsService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Ticket ticket)
    {
        await _ticketsService.CreateTicket(ticket);
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket?>> Get(long id)
    {
        var ticket = await _ticketsService.GetTicket(id);
        return ticket == null ? NotFound() : Ok(ticket);
    }
    [HttpPut("start")]
    public async Task<IActionResult> Start([FromBody] Ticket updatedTicket)
    {
        try
        {
            await _ticketsService.StartTicket(updatedTicket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
    [HttpPut("close")]
    public async Task<IActionResult> Close([FromBody] Ticket updatedTicket)
    {
        try
        {
            await _ticketsService.CloseTicket(updatedTicket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}
