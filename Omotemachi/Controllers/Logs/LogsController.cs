using Asp.Versioning;
using Omotemachi.Models.V1.Domain.Logs;
using Microsoft.AspNetCore.Mvc;
using Omotemachi.Services.Logs;
using Omotemachi.Models.V1.DTOs.Logs;

namespace Omotemachi.Controllers.Logs;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]")]
public class LogsController(ILogsService logsService) : ControllerBase
{
    private readonly ILogsService _logsService = logsService;

    [HttpPost("logs")]
    public async Task<IActionResult> CreateLog(LogDTO dto)
    {
        await _logsService.HandleAsync(dto);
        return Ok();
    }
}