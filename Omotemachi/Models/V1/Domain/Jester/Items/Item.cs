using Omotemachi.Models.V1.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Jester.Items;

public abstract class Item
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
}