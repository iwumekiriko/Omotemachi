using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester.Config;

public class TicketsConfig(long guildId) : IConfig
{
    [Key]
    public long GuildId { get; set; } = guildId;
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
    public long? TicketChannelId { get; set; }
    public long? TicketMessageId { get; set; }
    public long? TicketReportChannelId { get; set; }
}
