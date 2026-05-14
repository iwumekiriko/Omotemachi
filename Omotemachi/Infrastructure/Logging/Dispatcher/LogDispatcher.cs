using Humanizer;
using NuGet.Packaging;
using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Models.V1.Domain.Logs;
using LogLevel = Omotemachi.Models.V1.Domain.Logs.LogLevel;

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
    private static readonly Dictionary<LogLevel, int> EmbedColors = new()
    {
        [LogLevel.Debug] = 0x00bfff,
        [LogLevel.Information] = 0x90ee90,
        [LogLevel.Warning] = 0xffd700,
        [LogLevel.Error] = 0x8b0000,
        [LogLevel.Critical] = 0xff0000
    };

    private static readonly Dictionary<LogLevel, string> EmbedTitles = new()
    {
        [LogLevel.Debug] = "ДЕБАГ",
        [LogLevel.Information] = "ИНФОРМАЦИЯ",
        [LogLevel.Warning] = "ПРЕДУПРЕЖДЕНИЕ",
        [LogLevel.Error] = "ОШИБКА",
        [LogLevel.Critical] = "КРИТИЧЕСКАЯ ОШИБКА"
    };
    private const string DELETED_ATTACHMENTS_TITLE = "УДАЛЁННЫЕ ВЛОЖЕНИЯ";

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
        var containerComponents = new List<object>();

        var section = new
        {
            type = 9,
            components = new object[]
            {
                new
                {
                    type = 10,
                    content = $"## {EmbedTitles[log.Level]}\n\n{log.Message}"
                }
            },
            accessory = new
            {
                type = 11,
                media = new { url = log.AvatarUrl }
            }
        };

        containerComponents.Add(section);

        var separator = new
        {
            type = 14,
            spacing = 2,
            divider = true
        };
        var deletedAttachmentsTitle = new
        {
            type = 10,
            content = $"## {DELETED_ATTACHMENTS_TITLE}"
        };

        if (log.ImagesUrls.Length != 0)
        {
            var galleryItems = log.ImagesUrls
                .Select(url => new {
                    media = new { url },
                    spoiler = true 
                }).ToArray();

            var gallery = new
            {
                type = 12,
                items = galleryItems
            };

            containerComponents.Add(separator);
            containerComponents.Add(deletedAttachmentsTitle);
            containerComponents.Add(gallery);
        }

        var timestamp = new
        {
            type = 10,
            content = $"-# **{log.TimeStamp:dd MMMM yyyy — HH:mm}**"
        };
        containerComponents.Add(timestamp);

        var payload = new
        {
            flags = 32768,
            allowed_mentions = new { parse = Array.Empty<object>() },
            components = new object[]
            {
                new 
                {
                    type = 17,
                    accent_color = EmbedColors[log.Level],
                    components = containerComponents.ToArray()
                }
            }
        };

        try
        {
            var response = await client.PostAsJsonAsync(url + "?with_components=true", payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"Failed to send webhook. " +
                    $"Status: {(int)response.StatusCode} ({response.StatusCode})\n" +
                    $"Response: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook exception: {ex}");
        }
    }
}
