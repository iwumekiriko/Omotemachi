using Microsoft.EntityFrameworkCore;
using Omotemachi.Models.V1.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Statistics;

[PrimaryKey("UserId", "GuildId")]
public abstract class BaseStatistics
{
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }

    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }

    public abstract object GetStats();
}
