using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DellArteAPI.Models.V1.Jester;

public class Ticket
{
    public long Id { get; set; }
    public DateTime? DateCreate { get; set; }
    public DateTime? DateClose { get; set; }
    public string? DescriptionProblem { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Solution { get; set; }
    public string? TypeProblem { get; set; }
    public long? ModeratorId { get; set; }
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
    public long GuildId { get; set; }
    [ForeignKey("GuildId")]
    public Guild? Guild { get; set; }
}