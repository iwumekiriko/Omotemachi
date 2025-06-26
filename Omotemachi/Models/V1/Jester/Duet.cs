using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omotemachi.Models.V1.Jester;

public class Duet
{
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild Guild { get; set; }
    public long ProposerId { get; set; }
    [ForeignKey("ProposerId")]
    public User Proposer { get; set; }
    public long DuoId { get; set; }
    [ForeignKey("DuoId")]
    public User Duo { get; set; }
    public long TogetherFrom { get; set; }
    public bool Active { get; set; }
}