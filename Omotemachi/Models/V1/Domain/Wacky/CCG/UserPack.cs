using Omotemachi.Models.V1.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Domain.Wacky.CCG;

public class UserPack
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }

    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public int PackId { get; set; }
    [ForeignKey("PackId")]
    public Pack Pack { get; set; }

    public int Amount { get; set; } = 0;
}
