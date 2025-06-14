using DellArteAPI.Models.V1.Logs.Types;

namespace DellArteAPI.Models.V1.Logs;

public class WebLog : LogBase
{
    public WebLogType Type { get; set; }
}
