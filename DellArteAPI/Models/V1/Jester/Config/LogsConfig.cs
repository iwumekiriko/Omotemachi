using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Config;

public class LogsConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public string? CommandInteractionsWebhookUrl { get; set; }
    public string? MessagesWebhookUrl { get; set; }
    public string? TicketsWebhookUrl { get; set; }
    public string? GuildWebhookUrl { get; set; }
    public string? MembersWebhookUrl { get; set; }
    public string? VoiceWebhookUrl { get; set; }
    public string? ElseWebhookUrl { get; set; }
}
