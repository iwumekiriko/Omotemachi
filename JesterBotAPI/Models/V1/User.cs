using System.ComponentModel.DataAnnotations;

namespace DellArteAPI.Models.V1;

public class User
{
    [Key]
    public long Id { get; set; }
}