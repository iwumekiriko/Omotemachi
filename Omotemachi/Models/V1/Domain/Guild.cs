using System.ComponentModel.DataAnnotations;

namespace Omotemachi.Models.V1.Domain;

public class Guild
{
    [Key]
    public long Id { get; set; }
}