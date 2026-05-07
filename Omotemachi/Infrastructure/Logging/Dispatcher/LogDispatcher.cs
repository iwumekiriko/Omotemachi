using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Models.V1.Domain.Logs;

namespace Omotemachi.Infrastructure.Logging.Dispatcher;

public interface IWebhookDispatcher
{
    Task DispatchAsync(LogEntry log, LogsConfig config);
}

public class LogDispatcher(
    IHttpClientFactory httpFactory
) : IWebhookDispatcher
{
    private readonly IHttpClientFactory _httpFactory = httpFactory;

    public async Task DispatchAsync(LogEntry log, LogsConfig config)
    {
        if (log.GuildId == 0) return;

        var url = config.GetWebhookUrl(log.Category ?? "else");
        if (string.IsNullOrEmpty(url)) return;

        await SendWebhook(url, log);
    }

    private async Task SendWebhook(string url, LogEntry log)
    {
        var client = _httpFactory.CreateClient();

        var payload = new
        {
            content = $"[{log.Level}] {log.Message}"
        };

        await client.PostAsJsonAsync(url, payload);
    }
}
