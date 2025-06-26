using Omotemachi.Models.V1.Logs.Types;

namespace Omotemachi.Models.V1.Logs;

public class WebLog : LogBase
{
    public WebLogType Type { get; set; }
}
