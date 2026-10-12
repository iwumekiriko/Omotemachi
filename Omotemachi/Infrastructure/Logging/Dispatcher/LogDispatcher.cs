using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Models.V1.Domain.Logs;
using LogLevel = Omotemachi.Models.V1.Domain.Logs.LogLevel;

namespace Omotemachi.Infrastructure.Logging.Dispatcher;

public interface IWebhookDispatcher
{
    Task DiscordDispatchAsync(LogEntry log, LogsConfig config);
    Task VkDispatchAsync(LogEntry log);
}

public class LogDispatcher(
    IHttpClientFactory httpFactory,
    IConfiguration config
) : IWebhookDispatcher
{
    private readonly IHttpClientFactory _httpFactory = httpFactory;
    private readonly IConfiguration _config = config;
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

    public async Task DiscordDispatchAsync(LogEntry log, LogsConfig config)
    {
        if (log.GuildId == 0) return;

        var url = config.GetWebhookUrl(log.Category ?? "else");
        if (string.IsNullOrEmpty(url)) return;

        var payload = CreateDiscordPayload(log);
        await SendWebhook(url + "?with_components=true", payload);
    }
    public async Task VkDispatchAsync(LogEntry log)
    {
        var message = $"{EmbedTitles[log.Level]}\n\n{log.Message}";

        var queryParams = new Dictionary<string, string>
        {
            ["user_id"] = _config["vk_id"]!,
            ["message"] = message,
            ["random_id"] = log.Id.GetHashCode().ToString(),
            ["access_token"] = _config["vk_token"]!,
            ["v"] = "5.199"
        };

        var query = string.Join("&",
            queryParams.Select(x =>
                $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        var url = $"https://api.vk.com/method/messages.send?{query}";
        await SendWebhook(url);
    }

    private async Task SendWebhook(string url, object? payload = null)
    {
        var client = _httpFactory.CreateClient();

        try
        {
            var response = await client.PostAsJsonAsync(url, payload);

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
    private static object CreateDiscordPayload(LogEntry log)
    {
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

        return payload;
    }
}
