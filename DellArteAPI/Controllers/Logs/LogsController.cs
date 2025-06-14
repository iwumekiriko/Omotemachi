using Asp.Versioning;
using DellArteAPI.Models.V1.Logs;
using DellArteAPI.Models.V1.Logs.Types;
using Microsoft.AspNetCore.Mvc;

namespace DellArteAPI.Controllers.Logs;

[ApiController]
[ApiVersion(1)]
[Route("/api/v{version:apiVersion}/[controller]")]
public class LogsController : ControllerBase
{
    [HttpPost("jester/{type:int}/{guildId:long?}/{userId:long?}")]
    public async Task<IActionResult> WriteJesterLog(
        JesterLogType type,
        long? guildId,
        long? userId,
        [FromBody] LogBase log
    )
    {
        return Ok(new { Success = true, Status = StatusCodes.Status200OK });
    }
}
