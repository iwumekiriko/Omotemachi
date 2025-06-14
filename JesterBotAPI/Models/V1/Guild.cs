using System.ComponentModel.DataAnnotations;

namespace DellArteAPI.Models.V1;

public class Guild
{
    [Key]
    public long Id { get; set; }
}